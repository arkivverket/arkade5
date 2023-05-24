using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;
using ICSharpCode.SharpZipLib.Tar;
using System.IO;
using System.Text;

namespace Arkivverket.Arkade.Core.Util
{
    public class Noark5DocumentFileTarEntryTransferManager
    {
        public static void TransferDocumentFiles(Archive archive, string outputDirectoryFullPath)
        {
            var documentFiles = new Dictionary<string, DocumentFile>();

            string resultDirectoryFullPath = EnsureResultDirectoryIsCreated(archive.Uuid, outputDirectoryFullPath);

            using var tarInputStream = new TarInputStream(File.OpenRead(archive.ArchiveFileFullName), Encoding.UTF8);

            string packageFilePath = Path.Combine(resultDirectoryFullPath, archive.GetInformationPackageFileName());

            using Stream outStream = File.Create(packageFilePath);
            using var tarOutputStream = new TarOutputStream(outStream, Encoding.UTF8);

            while (tarInputStream.GetNextEntry() is { Name: not null } entry)
            {
                if (!entry.IsNoark5DocumentsEntry(archive.SourceUuid))
                    continue;

                tarOutputStream.PutNextEntry(entry);

                string relativePath = entry.GetRelativePathForNoark5DocumentEntry();

                if (archive.DocumentFilesAreRegistered)
                {
                    if (!archive.DocumentFilesHasCheckSums)
                    {
                        string checkSum = TransferDocumentFileAndGenerateSha256Checksum(tarInputStream, tarOutputStream);
                        archive.DocumentFiles[relativePath].CheckSum = checkSum;
                    }
                    else
                    {
                        TransferDocumentFile(tarInputStream, tarOutputStream);
                    }
                }
                else
                {
                    string checkSum = TransferDocumentFileAndGenerateSha256Checksum(tarInputStream, tarOutputStream);
                    DocumentFile documentFile = CreateDocumentFileFromEntry(entry, checkSum);
                    documentFiles.Add(relativePath, documentFile);
                }
                tarOutputStream.CloseEntry();
            }

            if (!archive.DocumentFilesAreRegistered)
                archive.SetDocumentFiles(new ReadOnlyDictionary<string, DocumentFile>(documentFiles));
        }

        private static void TransferDocumentFile(TarInputStream tarInputStream, Stream tarOutputStream)
        {
            tarInputStream.CopyEntryContents(tarOutputStream);
        }

        private static DocumentFile CreateDocumentFileFromEntry(TarEntry entry, string checksum)
        {
            var documentFile = new DocumentFile
            {
                Extension = Path.GetExtension(entry.Name).TrimStart("."),
                Size = entry.Size,
                CreationTime = entry.ModTime,
                CheckSum = checksum
            };

            return documentFile;
        }

        private static string TransferDocumentFileAndGenerateSha256Checksum(
            Stream tarInputStream, 
            Stream tarOutputStream)
        {
            var buffer = new byte[32 * 1024];

            var checksumGenerator = new Sha256ChecksumGenerator();

            checksumGenerator.Initialize();

            while (true)
            {
                int numRead = tarInputStream.Read(buffer, 0, buffer.Length);
                if (numRead <= 0)
                {
                    break;
                }

                checksumGenerator.TransformBlock(buffer, numRead);
                tarOutputStream.Write(buffer, 0, numRead);
            }

            return checksumGenerator.GenerateChecksum();
        }

        private static string EnsureResultDirectoryIsCreated(Uuid uuid, string outputDirectoryFullPath)
        {
            string resultDirectoryFullPath = Path.Combine(outputDirectoryFullPath,
                string.Format(OutputFileNames.ResultOutputDirectory, uuid));

            Directory.CreateDirectory(resultDirectoryFullPath);

            return resultDirectoryFullPath;
        }
    }
}
