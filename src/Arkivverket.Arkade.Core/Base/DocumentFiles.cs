using Arkivverket.Arkade.Core.Util;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Arkivverket.Arkade.Core.Base
{
    internal class DocumentFiles
    {
        private readonly Dictionary<string, DocumentFile> _documentFiles = new();

        private bool _haveCheckSums;
        private bool _areRegistered;

        private readonly DirectoryInfo _documentsDirectory;

        public int Count => _documentFiles.Count;

        public DocumentFiles(DirectoryInfo documentsDirectory)
        {
            _documentsDirectory = documentsDirectory;
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

        public void Register(bool includeChecksums)
        {
            if (_documentsDirectory != null)
            {
                if (!_areRegistered)
                    RegisterFromDirectory(includeChecksums);
                else if (_areRegistered && !_haveCheckSums && includeChecksums)
                    RegisterSha256Checksums();
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

        private void RegisterSha256Checksums()
        {
            IChecksumGenerator checksumGenerator = new Sha256ChecksumGenerator();

            foreach ((_, DocumentFile documentFile) in _documentFiles)
            {
                documentFile.CheckSum = checksumGenerator.GenerateChecksum(documentFile.FullName);
            }

            _haveCheckSums = true;
        }
    }
}
