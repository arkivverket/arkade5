using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;

namespace Arkivverket.Arkade.Util
{
    public class TarCompressionUtility : ICompressionUtility
    {
        // Tar file utilities built on nuget package SharpZipLib.0.86.0
        public void ExtractFolderFromArchive(string fileName, string targetFolderName)
        {
            Log.Debug($"Extract tar file: {fileName} to folder: {targetFolderName}");

            Stream inStream = File.OpenRead(fileName);
            var tarArchive = TarArchive.CreateInputTarArchive(inStream);
            tarArchive.ExtractContents(targetFolderName);
            tarArchive.Close();
            inStream.Close();
        }


        public void CompressFolderContentToArchiveFile(string targetFileName, string sourceFileFolder)
        {
            Stream outStream = File.Create(targetFileName);
            var tarOutputStream = new TarOutputStream(outStream);
            var tarArchive = TarArchive.CreateOutputTarArchive(tarOutputStream);

            // TODO: check difference between writing and not writing this 
            // Optionally, write an entry for the directory itself.
            //TarEntry tarEntry = TarEntry.CreateEntryFromFile(sourceFileFolder);
            //tarOutputStream.PutNextEntry(tarEntry);

            var filenames = Directory.GetFiles(sourceFileFolder, "*.*", SearchOption.AllDirectories);

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

                    var localBuffer = new byte[32*1024];
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
            tarOutputStream.Close();
        }
    }
}