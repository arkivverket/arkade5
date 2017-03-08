using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #45
    /// </summary>
    public class DocumentFilesChecksumControl : Noark5XmlReaderBaseTest
    {
        private string _currentArchivePartSystemId;
        private string _currentDocumentDescriptionSystemId;
        private DocumentObject _currentDocumentObject;
        private readonly DirectoryInfo _doucumentsDirectory;
        private readonly List<TestResult> _testResults;


        public DocumentFilesChecksumControl(Archive archive)
        {
            _doucumentsDirectory = archive.WorkingDirectory.Content()
                .WithSubDirectory(ArkadeConstants.DirectoryNameDocuments)
                .DirectoryInfo();

            _testResults = new List<TestResult>();
        }

        public override string GetName()
        {
            return Noark5Messages.DocumentFilesChecksumControl;
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
                _currentDocumentObject.NameReferencedFile = new FileInfo(eventArgs.Value).Name;

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
                        _testResults.Add(new TestResult(ResultType.Error,
                            new Location(ArkadeConstants.DirectoryNameDocuments),
                            string.Format(Noark5Messages.DocumentFilesChecksumControlMessage,
                                _currentDocumentObject.NameReferencedFile,
                                _currentDocumentObject.DocumentDescriptionSystemId
                            )));
                    }
                }
                catch (Exception)
                {
                    _testResults.Add(new TestResult(ResultType.Error,
                        new Location(ArkadeConstants.DirectoryNameDocuments),
                        string.Format(Noark5Messages.FileNotFound, _currentDocumentObject.NameReferencedFile)));
                }

                _currentDocumentObject = null;
                _currentDocumentDescriptionSystemId = null;
            }

            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePartSystemId = null;
        }

        private bool ActualAndDocumentedFileChecksumsMatch(DocumentObject documentObject)
        {
            var filePath = Path.Combine(_doucumentsDirectory.FullName, documentObject.NameReferencedFile);

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
            public string NameReferencedFile { get; set; }
            public string Checksum { get; set; }
            public string ChecksumAlgorithm { get; set; }
        }
    }
}
