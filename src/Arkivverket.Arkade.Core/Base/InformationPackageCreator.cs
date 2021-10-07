using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;

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
        public string CreateSip(TestSession testSession, string outputDirectory)
        {
            string packageFilePath = CreatePackage(PackageType.SubmissionInformationPackage, testSession, outputDirectory);

            return packageFilePath;
        }

        /// <summary>
        /// Create AIP (Archival Information Package)
        /// Package- and metafile are written to the given output directory
        /// The full path of the created package is returned
        /// </summary>
        public string CreateAip(TestSession testSession, string outputDirectory)
        {
            string packageFilePath = CreatePackage(PackageType.ArchivalInformationPackage, testSession, outputDirectory);

            return packageFilePath;
        }

        private string CreatePackage(PackageType packageType, TestSession testSession, string outputDirectory)
        {
            try
            {
                EnsureSufficientDiskSpace(testSession.Archive, outputDirectory);
            }
            catch
            {
                Log.Warning("Could not ensure sufficient disk space for package destination");
            }

            string resultDirectory = CreateResultDirectory(testSession.Archive, outputDirectory);

            if (packageType == PackageType.SubmissionInformationPackage && testSession.TestSuite.TestRuns.Any())
            {
                DirectoryInfo testReportDirectory = Directory.CreateDirectory(Path.Combine(
                    resultDirectory,
                    string.Format(OutputFileNames.StandaloneTestReportDirectory, testSession.Archive.Uuid)
                ));

                TestReportGeneratorRunner.RunAllGenerators(testSession, testReportDirectory, standalone: true);
            }
            else
            {
                DirectoryInfo testReportDirectory = Directory.CreateDirectory(testSession.Archive.GetTestReportDirectory().FullName);

                TestReportGeneratorRunner.RunAllGenerators(testSession, testReportDirectory, standalone: false);
            }

            string packageFilePath = Path.Combine(resultDirectory, testSession.Archive.GetInformationPackageFileName());

            using Stream outStream = File.Create(packageFilePath);
            using TarArchive tarArchive = TarArchive.CreateOutputTarArchive(new TarOutputStream(outStream));

            string packageRootDirectory = testSession.Archive.Uuid.GetValue() + Path.DirectorySeparatorChar;
            CreateEntry(packageRootDirectory, false, new DirectoryInfo("none"), tarArchive, string.Empty, string.Empty);

            AddFilesInDirectory(
                testSession.Archive, testSession.Archive.WorkingDirectory.Root().DirectoryInfo(), packageType, tarArchive, packageRootDirectory
            );

            if (testSession.Archive.WorkingDirectory.HasExternalContentDirectory())
            {
                Log.Debug($"Archive has external content directory, including files from {testSession.Archive.WorkingDirectory.Content()}");

                string contentDirectory = packageRootDirectory +
                                          ArkadeConstants.DirectoryNameContent +
                                          Path.DirectorySeparatorChar;

                AddFilesInDirectory(
                    testSession.Archive, testSession.Archive.WorkingDirectory.Content().DirectoryInfo(), null, tarArchive, contentDirectory
                );
            }

            tarArchive.Close();

            var diasMetsFilePath = Path.Combine(
                testSession.Archive.WorkingDirectory.Root().DirectoryInfo().FullName,
                ArkadeConstants.DiasMetsXmlFileName
            );

            new InfoXmlCreator().CreateAndSaveFile(testSession.ArchiveMetadata, packageFilePath, diasMetsFilePath,
                testSession.Archive.GetInfoXmlFileName());

            return packageFilePath;
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
            string fileNamePrefix = null)
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
            TarArchive tarArchive, string fileNamePrefix = null)
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

                CreateEntry(file.FullName, true, rootDirectory, tarArchive, fileNamePrefix);
            }
        }

        private void CreateEntry(string fileName, bool fileExists, DirectoryInfo rootDirectory, TarArchive tarArchive, string fileNamePrefix,
            string filenameSuffix = null)
        {
            TarEntry tarEntry = fileExists? TarEntry.CreateEntryFromFile(fileName) : TarEntry.CreateTarEntry(fileName);

            string packagePreparedFileName = fileNamePrefix +
                                             RemoveRootDirectoryFromFilename(fileName, rootDirectory.FullName) +
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
        SubmissionInformationPackage,
        ArchivalInformationPackage
    }
}