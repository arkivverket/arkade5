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
        ///     Create SIP (Submission Information Package)
        /// </summary>
        public void CreateSip(Archive archive)
        {
            CreatePackage(PackageType.SubmissionInformationPackage, archive);
        }

        /// <summary>
        ///     Create AIP (Archival Information Package)
        /// </summary>
        public void CreateAip(Archive archive)
        {
            CreatePackage(PackageType.ArchivalInformationPackage, archive);
        }

        private void CreatePackage(PackageType packageType, Archive archive)
        {
            Stream outStream = File.Create(archive.GetInformationPackageFileName().FullName);
            using (var tarOutputStream = new TarOutputStream(outStream))
            {
                TarArchive.CreateOutputTarArchive(tarOutputStream);

                string rootDirectory = archive.WorkingDirectory.Root().ToString();

                AddFilesInDirectory(rootDirectory, packageType, tarOutputStream);
                
                if (archive.WorkingDirectory.HasExternalContentDirectory())
                {
                    Log.Debug($"Archive has external content directory, include files from {archive.WorkingDirectory.Content()}");
                    string filenamePrefix = ArkadeConstants.DirectoryNameContent + Path.DirectorySeparatorChar;
                    AddFilesInDirectory(archive.WorkingDirectory.Content().ToString(), null, tarOutputStream, filenamePrefix);
                }
            }
        }

        private void AddFilesInDirectory(string rootDirectory, PackageType? packageType, TarOutputStream tarOutputStream, string filenamePrefix = null)
        {
            foreach (string directory in Directory.GetDirectories(rootDirectory))
            {
                CreateEntryForDirectory(directory, rootDirectory, tarOutputStream, filenamePrefix);
                string[] filenames = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

                foreach (string filename in filenames)
                {
                    if (packageType.HasValue && packageType == PackageType.SubmissionInformationPackage && FileIsInSkipList(filename))
                    {
                        Log.Debug($"Skipping {Path.GetFileName(filename)} for SIP-archive.");
                    }
                    else
                    {
                        AddFile(filename, rootDirectory, tarOutputStream, filenamePrefix);
                    }
                }
            }
        }

        private void CreateEntryForDirectory(string directory, string rootDirectory, TarOutputStream tarOutputStream, string filenamePrefix = null)
        {
            TarEntry tarEntry = TarEntry.CreateEntryFromFile(directory);
            tarEntry.Name = filenamePrefix + RemoveRootDirectoryFromFilename(directory, rootDirectory) + Path.DirectorySeparatorChar;
            tarOutputStream.PutNextEntry(tarEntry);
        }

        private static bool FileIsInSkipList(string fullPathToFile)
        {
            string fileName = Path.GetFileName(fullPathToFile);
            string lastDirectoryName = Path.GetFileName(Path.GetDirectoryName(fullPathToFile));
            return FilesToSkipForSipPackages.Contains(fileName)
                   || DirectoriesToSkipForSipPackages.Contains(lastDirectoryName);
        }

        private string RemoveRootDirectoryFromFilename(string filename, string rootDirectory)
        {
            return filename.Replace(rootDirectory + Path.DirectorySeparatorChar, "");
        }

        private void AddFile(string filename, string rootDirectory, TarOutputStream tarOutputStream, string filenamePrefix = null)
        {
            using (Stream inputStream = File.OpenRead(filename))
            {
                string entryName = filenamePrefix + RemoveRootDirectoryFromFilename(filename, rootDirectory);
                long fileSize = inputStream.Length;
                TarEntry entry = TarEntry.CreateTarEntry(entryName);
                entry.Size = fileSize;
                tarOutputStream.PutNextEntry(entry);

                var localBuffer = new byte[32*1024];
                while (true)
                {
                    int numRead = inputStream.Read(localBuffer, 0, localBuffer.Length);
                    if (numRead <= 0)
                    {
                        break;
                    }
                    tarOutputStream.Write(localBuffer, 0, numRead);
                }
            }
            tarOutputStream.CloseEntry();
        }
    }

    public enum PackageType
    {
        SubmissionInformationPackage,
        ArchivalInformationPackage
    }
}