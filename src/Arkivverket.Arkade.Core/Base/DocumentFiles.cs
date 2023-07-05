using Arkivverket.Arkade.Core.Util;
using ICSharpCode.SharpZipLib.Tar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Arkivverket.Arkade.Core.Base
{
    internal class DocumentFiles
    {
        private readonly Dictionary<string, DocumentFile> _documentFiles = new();

        private bool _haveCheckSums;
        private bool _areRegistered;

        private readonly DirectoryInfo _documentsDirectory;
        private readonly string _tarArchiveFullFileName;

        public int Count => _documentFiles.Count;

        public DocumentFiles(DirectoryInfo documentsDirectory)
        {
            _documentsDirectory = documentsDirectory;
        }

        public DocumentFiles(string tarArchiveFileFullName)
        {
            _tarArchiveFullFileName = tarArchiveFileFullName;
        }

        public ReadOnlyDictionary<string, DocumentFile> Get()
        {
            return new ReadOnlyDictionary<string, DocumentFile>(_documentFiles);
        }

        public static implicit operator ReadOnlyDictionary<string, DocumentFile>(DocumentFiles d) => d?.Get();

        public bool AreMetsReady()
        {
            return _areRegistered && _haveCheckSums;
        }

        public void TransferFromTarToInformationPackage(TarOutputStream tarOutputStream)
        {
            using var tarInputStream = new TarInputStream(File.OpenRead(_tarArchiveFullFileName), Encoding.UTF8);

            while (tarInputStream.GetNextEntry() is { Name: not null } entry)
            {
                if (!entry.IsNoark5DocumentsEntry(Path.GetFileNameWithoutExtension(_tarArchiveFullFileName)))
                    continue;

                tarOutputStream.PutNextEntry(entry);

                tarInputStream.CopyEntryContents(tarOutputStream);

                tarOutputStream.CloseEntry();
            }
        }

        public void Register(bool includeChecksums)
        {
            if (_documentsDirectory != null)
            {
                if (!_areRegistered)
                    RegisterFromDirectory(includeChecksums);
                else if (_areRegistered && !_haveCheckSums && includeChecksums)
                    RegisterSha256ChecksumsFromDirectory();
            }
            else if (_tarArchiveFullFileName != null)
            {
                if (!_areRegistered)
                    RegisterFromTar(includeChecksums);
                else if (_areRegistered && !_haveCheckSums && includeChecksums)
                    RegisterSha256ChecksumsFromTar();
            }
        }

        private void RegisterFromDirectory(bool includeChecksums)
        {
            if (!_documentsDirectory.Exists)
                return;

            IChecksumGenerator checksumGenerator = includeChecksums
                ? new Sha256ChecksumGenerator()
                : null;

            foreach (FileInfo documentFileInfo in _documentsDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                string relativePath = _documentsDirectory.Parent != null
                    ? Path.GetRelativePath(_documentsDirectory.Parent.FullName, documentFileInfo.FullName)
                    : documentFileInfo.FullName;

                string checkSum = includeChecksums
                    ? checksumGenerator.GenerateChecksum(documentFileInfo.FullName)
                    : null;

                var documentFile = new DocumentFile{
                    FullName = documentFileInfo.FullName,
                    Extension = documentFileInfo.Extension,
                    Size = documentFileInfo.Length,
                    CreationTime = documentFileInfo.CreationTime,
                    CheckSum = checkSum
                };

                _documentFiles.Add(relativePath.Replace('\\', '/'), documentFile);
            }

            if (includeChecksums)
                _haveCheckSums = true;

            _areRegistered = true;
        }

        private void RegisterSha256ChecksumsFromDirectory()
        {
            IChecksumGenerator checksumGenerator = new Sha256ChecksumGenerator();

            foreach ((_, DocumentFile documentFile) in _documentFiles)
            {
                documentFile.CheckSum = checksumGenerator.GenerateChecksum(documentFile.FullName);
            }

            _haveCheckSums = true;
        }

        /// <summary>
        /// This method traverses through all the entries in the archive file referred by
        /// <see cref="_tarArchiveFullFileName"/>, <br/> and registers all files from the "dokumenter"-directory. <br/>
        /// If <paramref name="includeChecksums"/> is <see langword="true"/>, SHA-256 checksum is generated for each
        /// file.
        /// </summary>
        /// <param name="includeChecksums"></param>
        private void RegisterFromTar(bool includeChecksums)
        {
            string tarRootDirectory = Path.GetFileNameWithoutExtension(_tarArchiveFullFileName);

            IChecksumGenerator checksumGenerator = includeChecksums
                ? new Sha256ChecksumGenerator()
                : null;

            using var tarInputStream = new TarInputStream(File.OpenRead(_tarArchiveFullFileName!), Encoding.UTF8);
            while (tarInputStream.GetNextEntry() is { Name: not null } entry)
            {
                if (!entry.IsNoark5DocumentsEntry(tarRootDirectory) || entry.IsDirectory)
                    continue;

                string checkSum = includeChecksums
                    ? tarInputStream.GenerateChecksumForEntry(checksumGenerator)
                    : null;

                var documentFile = new DocumentFile{
                    FullName = entry.Name,
                    Extension = Path.GetExtension(entry.Name).TrimStart("."),
                    Size = entry.Size,
                    CreationTime = entry.ModTime,
                    CheckSum = checkSum
                };

                string relativeEntryName = entry.GetRelativePathForNoark5DocumentEntry();

                _documentFiles.Add(relativeEntryName, documentFile);
            }

            if (includeChecksums)
                _haveCheckSums = true;

            _areRegistered = true;
        }

        private void RegisterSha256ChecksumsFromTar()
        {
            string tarRootDirectory = Path.GetFileNameWithoutExtension(_tarArchiveFullFileName);

            IChecksumGenerator checksumGenerator = new Sha256ChecksumGenerator();

            using var tarInputStream = new TarInputStream(File.OpenRead(_tarArchiveFullFileName!), Encoding.UTF8);

            while (tarInputStream.GetNextEntry() is { Name: not null } entry)
            {
                if (!entry.IsNoark5DocumentsEntry(tarRootDirectory) || entry.IsDirectory)
                    continue;

                string checkSum = tarInputStream.GenerateChecksumForEntry(checksumGenerator);

                string documentFileRelativePath = entry.GetRelativePathForNoark5DocumentEntry();

                _documentFiles[documentFileRelativePath].CheckSum = checkSum;
            }

            _haveCheckSums = true;
        }
    }
}
