using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;
using System.Runtime.Serialization;

namespace Arkivverket.Arkade.Core.Base
{
    public class InformationPackageCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<string> FilesToSkipForSipPackages = new List<string>
        {
            ArkadeConstants.EadXmlFileName,
            ArkadeConstants.EacCpfXmlFileName
        };

        private static readonly List<string> DirectoriesToSkipForSipPackages = new List<string>
        {
            ArkadeConstants.DirectoryNameRepositoryOperations
        };

        /// <summary>
        /// Create SIP (Submission Information Package). 
        /// Package- and metafile are written to the given output directory
        /// The full path of the created package is returned
        /// </summary>
        public string CreateSip(Archive archive, ArchiveMetadata metadata, string outputDirectory)
        {
            string packageFilePath = CreatePackage(PackageType.SubmissionInformationPackage, archive, metadata, outputDirectory);

            return packageFilePath;
        }

        /// <summary>
        /// Create AIP (Archival Information Package)
        /// Package- and metafile are written to the given output directory
        /// The full path of the created package is returned
        /// </summary>
        public string CreateAip(Archive archive, ArchiveMetadata metadata, string outputDirectory)
        {
            string packageFilePath = CreatePackage(PackageType.ArchivalInformationPackage, archive, metadata, outputDirectory);

            return packageFilePath;
        }

        private string CreatePackage(PackageType packageType, Archive archive, ArchiveMetadata metadata, string outputDirectory)
        {
            try
            {
                EnsureSufficientDiskSpace(archive, outputDirectory);
            }
            catch
            {
                Log.Warning("Could not verify sufficient disk space at package destination.");
            }

            string resultDirectory = CreateResultDirectory(archive, outputDirectory);

            if (packageType == PackageType.SubmissionInformationPackage)
            {
                CopyTestReportsToStandaloneDirectory(archive, resultDirectory);
            }

            string packageFilePath = Path.Combine(resultDirectory, archive.GetInformationPackageFileName());

            using Stream outStream = File.Create(packageFilePath);
            using var tarOutputStream = new TarOutputStream(outStream, Encoding.UTF8);
            using var tarArchive = TarArchive.CreateOutputTarArchive(tarOutputStream);

            string packageRootDirectory = archive.Uuid.GetValue() + Path.DirectorySeparatorChar;
            CreateEntry(packageRootDirectory, true, new DirectoryInfo("none"), tarArchive, string.Empty, string.Empty);

            AddFilesInDirectory(
                archive, archive.WorkingDirectory.Root().DirectoryInfo(), packageType, tarArchive, packageRootDirectory
            );

            if (archive.WorkingDirectory.HasExternalContentDirectory())
            {
                Log.Debug($"Archive has external content directory, including files from {archive.WorkingDirectory.Content()}");

                string contentDirectory = packageRootDirectory +
                                          ArkadeConstants.DirectoryNameContent +
                                          Path.DirectorySeparatorChar;

                AddFilesInDirectory(
                    archive, archive.WorkingDirectory.Content().DirectoryInfo(), null, tarArchive, contentDirectory
                );
            }

            if (archive.IsNoark5TarArchive)
                archive.DocumentFiles.TransferFromTarToInformationPackage(tarOutputStream);

            tarArchive.Close();

            var diasMetsFilePath = Path.Combine(
                archive.WorkingDirectory.Root().DirectoryInfo().FullName,
                ArkadeConstants.DiasMetsXmlFileName
            );

            new SubmissionDescriptionCreator().CreateAndSaveFile(metadata, packageFilePath, diasMetsFilePath,
                archive.GetSubmissionDescriptionFileName());

            return packageFilePath;
        }

        private void CopyTestReportsToStandaloneDirectory(Archive archive, string resultDirectory)
        {
            DirectoryInfo testReportDirectory = archive.GetTestReportDirectory();

            if (testReportDirectory.Exists)
            {
                FileInfo[] testReportFiles = testReportDirectory.GetFiles();

                if (testReportFiles.Any())
                {
                    DirectoryInfo testReportResultDirectory = Directory.CreateDirectory(Path.Combine(
                        resultDirectory, string.Format(OutputFileNames.StandaloneTestReportDirectory, archive.Uuid)
                    ));

                    foreach (FileInfo file in testReportFiles)
                    {
                        file.CopyTo(
                            Path.Combine(testReportResultDirectory.FullName,
                                file.Name.Equals(OutputFileNames.DbptkValidationReportFile)
                                    ? file.Name
                                    : string.Format(OutputFileNames.StandaloneTestReportFile, archive.Uuid,
                                        file.Extension.TrimStart('.'))),
                            overwrite: true);
                    }
                }
            }
        }

        private static void EnsureSufficientDiskSpace(Archive archive, string outputDirectory)
        {
            long driveSpace = SystemInfo.GetAvailableDiskSpaceInBytes(outputDirectory);
            long packageSize = archive.WorkingDirectory.GetSize();

            if (packageSize > driveSpace)
            {
                string errorMessage =
                    $"Insufficient disk space: Package size is {packageSize} bytes." +
                    $" Available space on destination drive is {driveSpace} bytes.";

                Log.Error(errorMessage);

                throw new InsufficientDiskSpaceException(errorMessage);
            }
        }
        
        private string CreateResultDirectory(Archive archive, string outputDirectory)
        {
            var resultDirectory = new DirectoryInfo(
                Path.Combine(outputDirectory, string.Format(OutputFileNames.ResultOutputDirectory, archive.Uuid))
            );

            resultDirectory.Create();

            return resultDirectory.FullName;
        }

        private void AddFilesInDirectory(Archive archive, DirectoryInfo rootDirectory, PackageType? packageType, TarArchive tarArchive,
            string fileNamePrefix)
        {
            AddFilesInDirectory(archive, rootDirectory, rootDirectory, packageType, tarArchive, fileNamePrefix);
        }

        /// <summary>
        ///     Recursively add all files and directories to the given tar archive.
        /// </summary>
        /// <param name="archive">the archive we are working on</param>
        /// <param name="directory">the directory we want to add files from</param>
        /// <param name="rootDirectory">this path is stripped from the filename used in tar file</param>
        /// <param name="packageType">the package type - used for filtering some files that are not needed for SIP-packages</param>
        /// <param name="tarArchive">the archive to add files to</param>
        /// <param name="fileNamePrefix">a prefix to add to all files after removing the root directory.</param>
        private void AddFilesInDirectory(Archive archive, DirectoryInfo directory, DirectoryInfo rootDirectory, PackageType? packageType,
            TarArchive tarArchive, string fileNamePrefix)
        {
            foreach (DirectoryInfo currentDirectory in directory.GetDirectories())
            {
                if ((packageType != null) && (packageType == PackageType.SubmissionInformationPackage) &&
                    DirectoriesToSkipForSipPackages.Contains(currentDirectory.Name))
                {     
                    continue;
                }
                
                CreateEntry(currentDirectory.FullName, true, rootDirectory, tarArchive, fileNamePrefix, Path.DirectorySeparatorChar.ToString());
                AddFilesInDirectory(archive, currentDirectory, rootDirectory, packageType, tarArchive, fileNamePrefix);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Name == archive.GetInformationPackageFileName()) // don't try to add the tar file into the tar file...
                {
                    continue;
                }

                if (FileIsInSkipList(packageType, file))
                {
                    continue;
                }

                CreateEntry(file.FullName, false, rootDirectory, tarArchive, fileNamePrefix);
            }
        }

        private void CreateEntry(string entryName, bool entryIsDirectory, DirectoryInfo rootDirectory, TarArchive tarArchive, string fileNamePrefix,
            string filenameSuffix = null)
        {
            TarEntry tarEntry;
            if (entryIsDirectory && !Directory.Exists(entryName))
            {
                tarEntry = TarEntry.CreateTarEntry(entryName);
                tarEntry.TarHeader.TypeFlag = TarHeader.LF_DIR;
            }
            else
            {
                tarEntry = TarEntry.CreateEntryFromFile(entryName);
            }

            string packagePreparedFileName = fileNamePrefix +
                                             RemoveRootDirectoryFromFilename(entryName, rootDirectory.FullName) +
                                             filenameSuffix;

            string tarEntryName = packagePreparedFileName.Replace("\\", "/"); // UNIX-style directory-separators

            tarEntry.Name = Encoding.GetEncoding("ISO-8859-1").GetString(Encoding.UTF8.GetBytes(tarEntryName));

            tarArchive.WriteEntry(tarEntry, false);
        }

        private string RemoveRootDirectoryFromFilename(string filename, string rootDirectory)
        {
            if (!rootDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                rootDirectory += Path.DirectorySeparatorChar;

            return filename.Replace(rootDirectory, "");
        }

        private static bool FileIsInSkipList(PackageType? packageType, FileInfo file)
        {
            return packageType.HasValue
                   && (packageType == PackageType.SubmissionInformationPackage)
                   && FilesToSkipForSipPackages.Contains(file.Name);
        }

        public static PackageType ParsePackageType(string packageType)
        {
            if (packageType.Equals("SIP", StringComparison.OrdinalIgnoreCase))
                return PackageType.SubmissionInformationPackage;

            if (packageType.Equals("AIP", StringComparison.OrdinalIgnoreCase))
                return PackageType.ArchivalInformationPackage;

            throw new ArgumentException(string.Format(ExceptionMessages.UnknownPackageType, packageType));
        }
    }

    public enum PackageType
    {
        [EnumMember(Value = "SIP")]
        SubmissionInformationPackage,
        [EnumMember(Value = "AIP")]
        ArchivalInformationPackage
    }
}