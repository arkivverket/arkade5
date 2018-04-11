using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class DocumentFilesChecksumControl : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 30);

        private string _currentArchivePartSystemId;
        private string _currentDocumentDescriptionSystemId;
        private DocumentObject _currentDocumentObject;
        private readonly List<TestResult> _testResults;
        private readonly DirectoryInfo _contentDirectory;


        public DocumentFilesChecksumControl(Archive archive)
        {
            _contentDirectory = archive.WorkingDirectory.Content().DirectoryInfo();

            _testResults = new List<TestResult>();
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.DocumentFilesChecksumControl;
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
                    ArchivePartSystemId = _currentArchivePartSystemId,
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
                _currentArchivePartSystemId = eventArgs.Value;

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
                _currentArchivePartSystemId = null;
        }

        private bool ActualAndDocumentedFileChecksumsMatch(DocumentObject documentObject)
        {
            var filePath = Path.Combine(_contentDirectory.FullName, documentObject.DocumentFileReference);

            var actualFileCheckSum = GenerateChecksumForFile(filePath, documentObject.ChecksumAlgorithm);
            return ChecksumsMatch(documentObject.Checksum, actualFileCheckSum);
        }

        private static string GenerateChecksumForFile(string filename, string checksumAlgorithm)
        {
            var generator = new ChecksumGeneratorFactory().GetGenerator(checksumAlgorithm);
            return generator.GenerateChecksum(filename);
        }

        private static bool ChecksumsMatch(string checksumA, string checkSumB)
        {
            return checksumA.ToUpper().Equals(checkSumB.ToUpper());
        }

        private class DocumentObject
        {
            public string ArchivePartSystemId { get; set; }
            public string DocumentDescriptionSystemId { get; set; }
            public string DocumentFileReference { get; set; }
            public string Checksum { get; set; }
            public string ChecksumAlgorithm { get; set; }
        }
    }
}
