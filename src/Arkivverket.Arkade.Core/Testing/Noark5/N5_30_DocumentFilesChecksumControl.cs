using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_30_DocumentFilesChecksumControl : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 30);

        private ArchivePart _currentArchivePart = new ArchivePart();
        private string _currentDocumentDescriptionSystemId;
        private DocumentObject _currentDocumentObject;
        private readonly List<TestResult> _testResults;
        private readonly ReadOnlyDictionary<string, DocumentFile> _documentFiles;


        public N5_30_DocumentFilesChecksumControl(Archive archive)
        {
            _documentFiles = archive.DocumentFiles;

            _testResults = new List<TestResult>();
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentobjekt"))
            {
                _currentDocumentObject = new DocumentObject
                {
                    ArchivePart = _currentArchivePart,
                    DocumentDescriptionSystemId = _currentDocumentDescriptionSystemId
                };
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("systemID", "dokumentbeskrivelse"))
                _currentDocumentDescriptionSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("referanseDokumentfil", "dokumentobjekt"))
                _currentDocumentObject.DocumentFileReference = eventArgs.Value;

            if (eventArgs.Path.Matches("sjekksum", "dokumentobjekt"))
                _currentDocumentObject.Checksum = eventArgs.Value;

            if (eventArgs.Path.Matches("sjekksumAlgoritme", "dokumentobjekt"))
                _currentDocumentObject.ChecksumAlgorithm = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentobjekt"))
            {
                try
                {
                    if (!ActualAndDocumentedFileChecksumsMatch(_currentDocumentObject))
                    {
                        _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                            string.Format(Noark5Messages.DocumentFilesChecksumControlMessage,
                                _currentArchivePart.SystemId,
                                _currentArchivePart.Name,
                                _currentDocumentObject.DocumentFileReference,
                                _currentDocumentObject.DocumentDescriptionSystemId
                            )));
                    }
                }
                catch (Exception)
                {
                    _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        string.Format(Noark5Messages.FileNotFound, _currentDocumentObject.DocumentFileReference)));
                }

                _currentDocumentObject = null;
                _currentDocumentDescriptionSystemId = null;
            }

            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        private bool ActualAndDocumentedFileChecksumsMatch(DocumentObject documentObject)
        {
            string documentFileRelativePath = documentObject.DocumentFileReference.Replace('\\', '/');

            var actualFileCheckSum = GetDocumentFileChecksum(documentFileRelativePath, documentObject.ChecksumAlgorithm);

            return ChecksumsMatch(documentObject.Checksum, actualFileCheckSum);
        }

        private string GetDocumentFileChecksum(string documentFileRelativePath, string checksumAlgorithm)
        {
            return _documentFiles[documentFileRelativePath]?.CheckSum != null
                ? _documentFiles[documentFileRelativePath].CheckSum
                : GenerateChecksumForFile(documentFileRelativePath, checksumAlgorithm);
        }

        private string GenerateChecksumForFile(string documentFileRelativePath, string checksumAlgorithm)
        {
            var documentFileFullName = _documentFiles[documentFileRelativePath]?.FileInfo?.FullName;

            var generator = new ChecksumGeneratorFactory().GetGenerator(checksumAlgorithm);
            string checkSum = generator.GenerateChecksum(documentFileFullName);

            _documentFiles[documentFileRelativePath].CheckSum = checkSum;

            return checkSum;
        }

        private static bool ChecksumsMatch(string checksumA, string checkSumB)
        {
            return checksumA.ToUpper().Equals(checkSumB.ToUpper());
        }

        private class DocumentObject
        {
            public ArchivePart ArchivePart { get; set; }
            public string DocumentDescriptionSystemId { get; set; }
            public string DocumentFileReference { get; set; }
            public string Checksum { get; set; }
            public string ChecksumAlgorithm { get; set; }
        }

    }
}
