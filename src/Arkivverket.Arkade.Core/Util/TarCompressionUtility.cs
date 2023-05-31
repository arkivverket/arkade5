using System.IO;
using System;
using System.Text;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;

namespace Arkivverket.Arkade.Core.Util
{
    /// <summary>
    ///  Using SharpZipLib for tar operations
    /// </summary>
    public class TarCompressionUtility : ICompressionUtility
    {
        private readonly ILogger _log = Log.ForContext<TarCompressionUtility>();

        public void ExtractFolderFromArchive(FileInfo file, DirectoryInfo targetDirectory, bool withoutDocumentFiles, 
            string archiveRootDirectoryName)
        {
            using (var inputStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                DirectoryInfo singleRootDirectory = GetSingleRootDirectory(inputStream);
                inputStream.Position = 0; // Needs resetting after GetSingleRootDirectory()

                var tarInputStream = new TarInputStream(inputStream, Encoding.UTF8);

                while (tarInputStream.GetNextEntry() is { } tarEntry)
                {
                    if (withoutDocumentFiles && tarEntry.IsNoark5DocumentsEntry(archiveRootDirectoryName))
                        continue;

                    if (tarEntry.IsDirectory)
                        continue;

                    string name = tarEntry.Name.Replace('/', Path.DirectorySeparatorChar);

                    if (singleRootDirectory != null)
                        name = name[singleRootDirectory.Name.Length..];

                    if (Path.IsPathRooted(name))
                        name = name[Path.GetPathRoot(name).Length..];

                    string fullName = Path.Combine(targetDirectory.FullName, name);
                    string directoryName = Path.GetDirectoryName(fullName);
                    if (directoryName != null) Directory.CreateDirectory(directoryName);

                    var outputStream = new FileStream(fullName, FileMode.Create);

                    tarInputStream.CopyEntryContents(outputStream);
                    outputStream.Close();

                    DateTime dateTime = DateTime.SpecifyKind(tarEntry.ModTime, DateTimeKind.Utc);
                    File.SetLastWriteTime(fullName, dateTime);
                }
                tarInputStream.Close();
            }
            _log.Debug($"Extracted tar file: {file} to folder {targetDirectory}", file.FullName, targetDirectory.FullName);
        }

        private static DirectoryInfo GetSingleRootDirectory(Stream inputStream)
        {
            var tarInputStream = new TarInputStream(inputStream, Encoding.UTF8);
            TarEntry firstEntry = tarInputStream.GetNextEntry();

            if (!firstEntry.IsDirectory)
                return null;

            while (tarInputStream.GetNextEntry() is { } tarEntry)
                if (!tarEntry.Name.StartsWith(firstEntry.Name))
                    return null;

            return new DirectoryInfo(firstEntry.Name);
        }

        public void CompressFolderContentToArchiveFile(FileInfo targetFileName, DirectoryInfo sourceFileFolder)
        {
            Stream outStream = File.Create(targetFileName.FullName);
            var tarOutputStream = new TarOutputStream(outStream, Encoding.UTF8);

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