using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;

namespace Arkivverket.Arkade.Util
{
    /// <summary>
    ///  Using SharpZipLib for tar operations
    /// </summary>
    public class TarCompressionUtility : ICompressionUtility
    {
        private readonly ILogger _log = Log.ForContext<TarCompressionUtility>();

        public void ExtractFolderFromArchive(FileInfo file, DirectoryInfo targetDirectory)
        {
            Stream inStream = File.OpenRead(file.FullName);
            var tarArchive = TarArchive.CreateInputTarArchive(inStream);
            tarArchive.ExtractContents(targetDirectory.FullName);
            tarArchive.Close();
            inStream.Close();

            _log.Debug($"Extracted tar file: {file} to folder {targetDirectory}", file.FullName, targetDirectory.FullName);
        }


        public void CompressFolderContentToArchiveFile(FileInfo targetFileName, DirectoryInfo sourceFileFolder)
        {
            Stream outStream = File.Create(targetFileName.FullName);
            var tarOutputStream = new TarOutputStream(outStream);

            // TODO: check difference between writing and not writing this 
            // Optionally, write an entry for the directory itself.
            //TarEntry tarEntry = TarEntry.CreateEntryFromFile(sourceFileFolder);
            //tarOutputStream.PutNextEntry(tarEntry);

            var filenames = Directory.GetFiles(sourceFileFolder.FullName, "*.*", SearchOption.AllDirectories);

            // Remove source file path from filename and add each file to the TAR acrchive
            foreach (var filename in filenames)
            {
                using (Stream inputStream = File.OpenRead(filename))
                {
                    var tarName = filename.Replace(sourceFileFolder.FullName, "");
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