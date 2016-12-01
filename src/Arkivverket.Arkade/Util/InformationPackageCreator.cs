using System.IO;
using Arkivverket.Arkade.Core;
using ICSharpCode.SharpZipLib.Tar;

namespace Arkivverket.Arkade.Util
{
    public class InformationPackageCreator
    {

        /// <summary>
        /// Create SIP (Submission Information Package)
        /// </summary>
        public void CreateSip(Archive archive, string targetFileName)
        {
            string sipDirectory = PrepareSipDirectoriesAndFiles(archive.WorkingDirectory.Root());



            Stream outStream = File.Create(targetFileName);
            using (var tarOutputStream = new TarOutputStream(outStream))
            {
                var tarArchive = TarArchive.CreateOutputTarArchive(tarOutputStream);

                foreach (var directory in System.IO.Directory.GetDirectories(sipDirectory))
                {
                    TarEntry tarEntry = TarEntry.CreateEntryFromFile(directory);
                    tarEntry.Name = tarEntry.Name.Replace(sipDirectory.Replace("\\","/") + "/", "");
                    tarOutputStream.PutNextEntry(tarEntry);

                    AddFilesInFolder(directory, tarOutputStream);
                }
            }
        }

        private void AddFilesInFolder(string sourceFileFolder, TarOutputStream tarOutputStream)
        {
            var filenames = System.IO.Directory.GetFiles(sourceFileFolder, "*.*", SearchOption.AllDirectories);
            // Remove source file path from filename and add each file to the TAR acrchive
            foreach (var filename in filenames)
            {
                using (Stream inputStream = File.OpenRead(filename))
                {
                    var tarName = filename.Replace(sourceFileFolder, "");
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


        private string PrepareSipDirectoriesAndFiles(ArkadeDirectory archiveWorkingDirectory)
        {
            string sipDirectory = archiveWorkingDirectory + "sip";
            System.IO.Directory.CreateDirectory(sipDirectory + "\\content");

            DirectoryInfo descriptiveMetadata = System.IO.Directory.CreateDirectory(sipDirectory + "\\descriptive_metadata");
          //  File.Move(archiveWorkingDirectory + "", );


            DirectoryInfo administrativeMetadata = System.IO.Directory.CreateDirectory(sipDirectory + "\\administrative_metadata");

            return sipDirectory;
        }

        /// <summary>
        /// Create AIP (Archival Information Package)
        /// </summary>
        public void CreateAip()
        {
            
        }

    }
}
