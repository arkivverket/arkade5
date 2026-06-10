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
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Report;

namespace Arkivverket.Arkade.Core.Base
{
    public class InformationPackageCreator(MetadataFilesCreator metadataFilesCreator, IStatusEventHandler statusEventHandler, SiardMetadataFileHelper siardMetadataFileHelper, TestSessionXmlGenerator testSessionXmlGenerator)
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Create SIP (Submission Information Package). 
        /// Package- and metafile are written to the given output directory
        /// The full path of the created package is returned
        /// </summary>
        public string CreateSip(Archive archive, string outputDirectory)
        {
            string packageFilePath = CreatePackage(archive, outputDirectory);

            return packageFilePath;
        }

        /// <summary>
        /// Create AIP (Archival Information Package)
        /// Package- and metafile are written to the given output directory
        /// The full path of the created package is returned
        /// </summary>
        public string CreateAip(Archive archive, string outputDirectory)
        {
            string packageFilePath = CreatePackage(archive, outputDirectory);

            return packageFilePath;
        }

        private string CreatePackage(Archive archive, string outputDirectoryPath) // TODO: Generate and collect all files from/to the right places
        {
            OutputDiasPackage outputDiasPackage = archive.OutputDiasPackage;

            if (archive.TestSession?.TestSuite != null)
            {
                var outputDirectory = new DirectoryInfo(outputDirectoryPath);
                
                TestReportGeneratorRunner.RunAllGenerators(archive, outputDirectory, 100 /*TODO: fix!*/, outputDiasPackage, out DirectoryInfo reportsDirectory);
        
                if (archive is SiardArchive)
                    File.Copy(
                        sourceFileName: Path.Combine(archive.TestSession.TemporaryTestResultFilesDirectory.FullName, OutputFileNames.DbptkValidationReportFile),
                        destFileName: Path.Combine(reportsDirectory.FullName, OutputFileNames.DbptkValidationReportFile),
                        overwrite: true
                    );

                // Ship the test-session log inside the package (AIP only; SIP omits repository_operations),
                // freshly written here so it carries the output package's UUID.
                if (outputDiasPackage.PackageType == PackageType.ArchivalInformationPackage)    
                    testSessionXmlGenerator.GenerateXmlAndSaveToFile(archive, outputDiasPackage);
            }
            
            if (archive is (Noark5Archive or SpecializedSystemArchive) and AddmlBasedArchive { AddmlXmlUnit: not null } addmlBasedArchive)
                addmlBasedArchive.AddmlXmlUnit.WriteFiles(outputDiasPackage.WorkingDirectory.AdministrativeMetadata());
            
            if(archive is SiardArchive siardArchive)
            {
                siardMetadataFileHelper.ExtractSiardMetadataFilesToAdministrativeMetadata(siardArchive);
            }
            
            metadataFilesCreator.Create(archive);
            
            try
            {
                EnsureSufficientDiskSpace(archive, outputDirectoryPath);
            }
            catch
            {
                Log.Warning("Could not verify sufficient disk space at package destination.");
            }

            string resultDirectory = CreateResultDirectory(outputDiasPackage.Id, outputDirectoryPath);

            string packageFilePath = Path.Combine(resultDirectory, outputDiasPackage.Id + ".tar"); // NB! UUID-writeout (package creation)

            using Stream outStream = File.Create(packageFilePath);
            using var tarOutputStream = new TarOutputStream(outStream, Encoding.UTF8);
            using var tarArchive = TarArchive.CreateOutputTarArchive(tarOutputStream);

            string packageRootDirectory = outputDiasPackage.Id.GetValue() + Path.DirectorySeparatorChar; // NB! UUID-writeout (package creation)
            CreateEntry(packageRootDirectory, true, new DirectoryInfo("none"), tarArchive, string.Empty, string.Empty);

            AddFilesInDirectory(
                outputDiasPackage, outputDiasPackage.WorkingDirectory.Root().DirectoryInfo(), outputDiasPackage.PackageType, tarArchive, packageRootDirectory
            );

            // Stream the archive content straight into the package's content directory. The content is
            // read in place from its source (Archive.Content) rather than being staged in the work
            // directory, so large extractions are never copied to disk. The empty content directory
            // created under the work directory (see DiasPackageWorkingDirectory.CreateDirectories)
            // guarantees the package always contains a content directory, even with no content files.
            {
                Log.Debug($"Writing archive content to the package content directory from {archive.Content}");

                var contentDirectoryPath = $"{outputDiasPackage.Id}/{ArkadeConstants.DirectoryNameContent}";

                foreach ((FileSystemInfo contentItem, string contentRelativeFilePath) in archive.Content.Get())
                {
                    var tarEntry = TarEntry.CreateEntryFromFile(contentItem.FullName);
                    tarEntry.Name = $"{contentDirectoryPath}/{contentRelativeFilePath}";
                    tarArchive.WriteEntry(tarEntry, false);
                }
            }

            if (archive is Noark5Archive { InputDiasPackage.TarFile: not null } noark5Archive )
                noark5Archive.DocumentFiles.TransferFromTarToInformationPackage(tarOutputStream, packageRootDirectory);

            tarArchive.Close();

            var diasMetsFilePath = Path.Combine(
                archive.OutputDiasPackage.WorkingDirectory.Root().DirectoryInfo().FullName,
                ArkadeConstants.DiasMetsXmlFileName
            );

            ArchiveMetadata metadataForSubmissionDescription = outputDiasPackage.ArchiveMetadata.Clone();

            new SubmissionDescriptionCreator().CreateAndSaveFile(metadataForSubmissionDescription, packageFilePath, diasMetsFilePath,
                outputDiasPackage.Id + ".xml"); // NB! UUID-writeout (package creation)

            return packageFilePath;
        }

        private static void EnsureSufficientDiskSpace(Archive archive, string outputDirectory)
        {
            long driveSpace = SystemInfo.GetAvailableDiskSpaceInBytes(outputDirectory);
            long packageSize = EstimatePackageSize(archive);

            if (packageSize > driveSpace)
            {
                string errorMessage =
                    $"Insufficient disk space: Package size is {packageSize} bytes." +
                    $" Available space on destination drive is {driveSpace} bytes.";

                Log.Error(errorMessage);

                throw new InsufficientDiskSpaceException(errorMessage);
            }
        }

        private static long EstimatePackageSize(Archive archive)
        {
            // Work-directory staged files (package metadata and similar) plus the archive content, which is
            // streamed into the package from its source rather than staged in the work directory (see
            // CreatePackage). Each archive type knows how to size its own content (see Archive.GetContentSize).
            return archive.OutputDiasPackage.WorkingDirectory.GetSize() + archive.GetContentSize();
        }
        
        private string CreateResultDirectory(Uuid informationPackageUuid, string outputDirectory)
        {
            var resultDirectory = new DirectoryInfo(
                Path.Combine(outputDirectory, string.Format(OutputFileNames.ResultOutputDirectory, informationPackageUuid)) // NB! UUID-writeout (package creation)
            );

            resultDirectory.Create();

            return resultDirectory.FullName;
        }

        private void AddFilesInDirectory(OutputDiasPackage diasPackage, DirectoryInfo rootDirectory, PackageType? packageType, TarArchive tarArchive,
            string fileNamePrefix)
        {
            AddFilesInDirectory(diasPackage, rootDirectory, rootDirectory, packageType, tarArchive, fileNamePrefix);
        }

        /// <summary>
        ///     Recursively add all files and directories to the given tar archive.
        /// </summary>
        /// <param name="diasPackage">the information package we are working on</param>
        /// <param name="directory">the directory we want to add files from</param>
        /// <param name="rootDirectory">this path is stripped from the filename used in tar file</param>
        /// <param name="packageType">the package type - used for filtering some files that are not needed for SIP-packages</param>
        /// <param name="tarArchive">the archive to add files to</param>
        /// <param name="fileNamePrefix">a prefix to add to all files after removing the root directory.</param>
        private void AddFilesInDirectory(OutputDiasPackage diasPackage, DirectoryInfo directory, DirectoryInfo rootDirectory, PackageType? packageType,
            TarArchive tarArchive, string fileNamePrefix)
        {
            foreach (DirectoryInfo currentDirectory in directory.GetDirectories())
            {
                CreateEntry(currentDirectory.FullName, true, rootDirectory, tarArchive, fileNamePrefix, Path.DirectorySeparatorChar.ToString());
                AddFilesInDirectory(diasPackage, currentDirectory, rootDirectory, packageType, tarArchive, fileNamePrefix);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Name == diasPackage.Id + ".tar") // don't try to add the tar file into the tar file...  // NB! UUID-writeout (package creation)
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

            tarEntry.Name = packagePreparedFileName.Replace("\\", "/"); // UNIX-style directory-separators

            tarArchive.WriteEntry(tarEntry, false);
        }

        private string RemoveRootDirectoryFromFilename(string filename, string rootDirectory)
        {
            if (!rootDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                rootDirectory += Path.DirectorySeparatorChar;

            return filename.Replace(rootDirectory, "");
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