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

        private ArchivePart _currentArchivePart = new();
        private string _currentDocumentDescriptionSystemId;
        private DocumentObject _currentDocumentObject;
        private readonly TestResultSet _testResultSet;
        private readonly ReadOnlyDictionary<string, DocumentFile> _documentFiles;


        public N5_30_DocumentFilesChecksumControl(Archive archive)
        {
            _documentFiles = archive.DocumentFiles;

            _testResultSet = new TestResultSet();
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override TestResultSet GetTestResults()
        {
            if (_testResultSet.TestResultSets.Count != 1)
                return _testResultSet;

            _testResultSet.TestsResults = _testResultSet.TestResultSets[0].TestsResults;
            _testResultSet.TestResultSets = new List<TestResultSet>();
            return _testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentobjekt"))
            {
                _currentDocumentObject = new DocumentObject
                {
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
            {
                _currentDocumentObject.DocumentFileReference = eventArgs.Value;
                _currentDocumentObject.DocumentFileReferenceXmlLineNumber = eventArgs.LineNumber;
            }

            if (eventArgs.Path.Matches("sjekksum", "dokumentobjekt"))
            {
                _currentDocumentObject.Checksum = eventArgs.Value;
                _currentDocumentObject.ChecksumXmlLineNumber = eventArgs.LineNumber;
            }

            if (eventArgs.Path.Matches("sjekksumAlgoritme", "dokumentobjekt"))
                _currentDocumentObject.ChecksumAlgorithm = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentobjekt"))
            {
                TestResultSet archivePartResultSet =
                    _testResultSet.TestResultSets.Find(s => s.Name.Contains(_currentArchivePart.SystemId));

                TestResult testResult = null;

                try
                {
                    if (!ActualAndDocumentedFileChecksumsMatch(_currentDocumentObject))
                    {
                        testResult = new TestResult(ResultType.Error, new Location(
                                ArkadeConstants.ArkivuttrekkXmlFileName, _currentDocumentObject.ChecksumXmlLineNumber),
                            string.Format(Noark5Messages.DocumentFilesChecksumControlMessage,
                                _currentDocumentObject.DocumentFileReference,
                                _currentDocumentObject.DocumentDescriptionSystemId
                            ));
                    }
                }
                catch (Exception)
                {
                    testResult = new TestResult(ResultType.Error, new Location(ArkadeConstants.ArkivuttrekkXmlFileName,
                        _currentDocumentObject.DocumentFileReferenceXmlLineNumber), string.Format(
                        Noark5Messages.FileNotFound, _currentDocumentObject.DocumentFileReference));
                }

                if (testResult != null)
                {
                    if (archivePartResultSet != null)
                        archivePartResultSet.TestsResults.Add(testResult);

                    else
                        _testResultSet.TestResultSets.Add(new TestResultSet
                        {
                            Name = _currentArchivePart.ToString(),
                            TestsResults = new List<TestResult>
                            {
                                testResult
                            }
                        });
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
            public string DocumentDescriptionSystemId { get; set; }
            public string DocumentFileReference { get; set; }
            public string Checksum { get; set; }
            public string ChecksumAlgorithm { get; set; }
            public long DocumentFileReferenceXmlLineNumber { get; set; }
            public long ChecksumXmlLineNumber { get; set; }
        }

    }
}
