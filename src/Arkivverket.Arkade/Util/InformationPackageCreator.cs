using System;
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
            ArkadeConstants.DirectoryNameRepositoryOperations,
        };

        /// <summary>
        /// Create SIP (Submission Information Package)
        /// </summary>
        public void CreateSip(Archive archive)
        {
            CreatePackage(PackageType.SubmissionInformationPackage, archive);
        }

        /// <summary>
        /// Create AIP (Archival Information Package)
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
                foreach (var directory in Directory.GetDirectories(rootDirectory))
                {
                    var filenames = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

                    foreach (var filename in filenames)
                    {
                        if (packageType == PackageType.SubmissionInformationPackage
                            && FileIsInSkipList(filename))
                            Log.Debug($"Skipping {Path.GetFileName(filename)} for SIP-archive.");
                        else
                            AddFile(filename, rootDirectory, tarOutputStream);
                    }
                }
            }
        }

        private static bool FileIsInSkipList(string fullPathToFile)
        {
            string fileName = Path.GetFileName(fullPathToFile);
            string lastDirectoryName = Path.GetFileName(Path.GetDirectoryName(fullPathToFile));
            return FilesToSkipForSipPackages.Contains(fileName) 
                || DirectoriesToSkipForSipPackages.Contains(lastDirectoryName);
        }

        private void AddFile(string filename, string sourceFileFolder, TarOutputStream tarOutputStream)
        {
            using (Stream inputStream = File.OpenRead(filename))
            {
                // Remove source file path from filename
                var tarName = filename.Replace(sourceFileFolder + Path.DirectorySeparatorChar, "");
                var fileSize = inputStream.Length;
                var entry = TarEntry.CreateTarEntry(tarName);
                entry.Size = fileSize;
                tarOutputStream.PutNextEntry(entry);

                var localBuffer = new byte[32 * 1024];
                while (true)
                {
                    var numRead = inputStream.Read(localBuffer, 0, localBuffer.Length);
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
        SubmissionInformationPackage, ArchivalInformationPackage
    }
}
