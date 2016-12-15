using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;

namespace Arkivverket.Arkade.Util
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
        /// Output file is written to Arkade's work directory for this test session
        /// </summary>
        public void CreateSip(Archive archive)
        {
            CreatePackage(PackageType.SubmissionInformationPackage, archive);
        }

        /// <summary>
        /// Create AIP (Archival Information Package)
        /// Output file is written to Arkade's work directory for this test session
        /// </summary>
        public void CreateAip(Archive archive)
        {
            CreatePackage(PackageType.ArchivalInformationPackage, archive);
        }

        private void CreatePackage(PackageType packageType, Archive archive)
        {
            Stream outStream = File.Create(archive.GetInformationPackageFileName().FullName);
            TarArchive tarArchive = TarArchive.CreateOutputTarArchive(new TarOutputStream(outStream));

            AddFilesInDirectory(archive, archive.WorkingDirectory.Root().DirectoryInfo(), packageType, tarArchive);

            if (archive.WorkingDirectory.HasExternalContentDirectory())
            {
                Log.Debug($"Archive has external content directory, including files from {archive.WorkingDirectory.Content()}");
                string filenamePrefix = ArkadeConstants.DirectoryNameContent + Path.DirectorySeparatorChar;
                AddFilesInDirectory(archive, archive.WorkingDirectory.Content().DirectoryInfo(), null, tarArchive, filenamePrefix);
            }

            tarArchive.Close();
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

                CreateEntry(currentDirectory.FullName, rootDirectory, tarArchive, fileNamePrefix, Path.DirectorySeparatorChar.ToString());
                AddFilesInDirectory(archive, currentDirectory, rootDirectory, packageType, tarArchive, fileNamePrefix);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.FullName == archive.GetInformationPackageFileName().FullName) // don't try to add the tar file into the tar file...
                {
                    continue;
                }

                if (FileIsInSkipList(packageType, file))
                {
                    continue;
                }

                CreateEntry(file.FullName, rootDirectory, tarArchive, fileNamePrefix);
            }
        }

        private void CreateEntry(string fileName, DirectoryInfo rootDirectory, TarArchive tarArchive, string fileNamePrefix,
            string filenameSuffix = null)
        {
            TarEntry tarEntry = TarEntry.CreateEntryFromFile(fileName);
            tarEntry.Name = fileNamePrefix + RemoveRootDirectoryFromFilename(fileName, rootDirectory.FullName) + filenameSuffix;
            tarArchive.WriteEntry(tarEntry, false);
        }

        private string RemoveRootDirectoryFromFilename(string filename, string rootDirectory)
        {
            return filename.Replace(rootDirectory + Path.DirectorySeparatorChar, "");
        }

        private static bool FileIsInSkipList(PackageType? packageType, FileInfo file)
        {
            return packageType.HasValue
                   && (packageType == PackageType.SubmissionInformationPackage)
                   && FilesToSkipForSipPackages.Contains(file.Name);
        }
    }

    public enum PackageType
    {
        SubmissionInformationPackage,
        ArchivalInformationPackage
    }
}