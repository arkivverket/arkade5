using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_29_NumberOfEachDocumentFormat : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 29);

        private readonly Dictionary<string, int> _numberOfUniqueDocumentFormats = new();
        private readonly Dictionary<ArchivePart, List<DocumentObject>> _documentObjectsPerArchivePart = new();
        private ArchivePart _currentArchivePart = new ArchivePart();
        private DocumentObject _currentDocumentObject;

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override TestResultSet GetTestResults()
        {
            bool multipleArchiveParts = _documentObjectsPerArchivePart.Count > 1;
            int totalNumberOfUniqueDocumentFormats = _numberOfUniqueDocumentFormats.Count;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new (ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfUniqueDocumentFormats, totalNumberOfUniqueDocumentFormats))
                }
            };

            if (totalNumberOfUniqueDocumentFormats == 0)
                return testResultSet;

            var totalNumberOfFoldersWithFormatMismatch = 0;

            foreach ((ArchivePart archivePart, List<DocumentObject> documentObjects) in _documentObjectsPerArchivePart)
            {
                if (documentObjects.Count(d => d.Format != default && d.FileReference != default) == 0)
                {
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = new List<TestResult>
                        {
                            new(ResultType.Success, new Location(string.Empty), string.Format(
                                Noark5Messages.TotalResultNumber, 0))
                        }
                    });
                    continue;
                }
                var numberOfDocumentsWithFormatMismatch = 0;

                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                };

                foreach (IGrouping<string, DocumentObject> group in documentObjects.GroupBy(d => d.Format,
                    StringComparer.OrdinalIgnoreCase))
                {
                    var numberOfFormatMismatches = 0;
                    var testResults = new List<TestResult>
                    {
                        new(ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.NumberOf, group.Count()))
                    };

                    var errorResults = new List<TestResult>();

                    foreach (DocumentObject documentObject in group.Where(d => d.HasFormatMismatch()))
                    {
                        errorResults.Add(new TestResult(ResultType.Error, 
                            new Location(ArkadeConstants.ArkivuttrekkXmlFileName, documentObject.XmlLineNumber),
                            string.Format(Noark5Messages.NumberOfEachDocumentFormatMessage_FormatMismatch,
                                documentObject.FileReference)));

                        numberOfFormatMismatches++;
                    }

                    if (numberOfFormatMismatches > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOfDocumentsWithFormatMismatch,
                                numberOfFormatMismatches)));
                        testResults.AddRange(errorResults);
                    }

                    var documentFormatResultSet = new TestResultSet
                    {
                        Name = string.Format(Noark5Messages.DocumentFormatMessage, @group.Key),
                        TestsResults = testResults
                    };

                    if (multipleArchiveParts)
                        archivePartResultSet.TestResultSets.Add(documentFormatResultSet);
                    else
                        testResultSet.TestResultSets.Add(documentFormatResultSet);

                    numberOfDocumentsWithFormatMismatch += numberOfFormatMismatches;
                }

                if (multipleArchiveParts)
                {
                    if (numberOfDocumentsWithFormatMismatch > 0)
                        archivePartResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                            new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOfDocumentsWithFormatMismatch,
                                numberOfDocumentsWithFormatMismatch)));

                    testResultSet.TestResultSets.Add(archivePartResultSet);
                }

                totalNumberOfFoldersWithFormatMismatch += numberOfDocumentsWithFormatMismatch;
            }

            if (totalNumberOfFoldersWithFormatMismatch > 0)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfDocumentsWithFormatMismatch,
                        totalNumberOfFoldersWithFormatMismatch)));

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("dokumentobjekt", "dokumentbeskrivelse", "registrering"))
                _currentDocumentObject = new DocumentObject(eventArgs.LineNumber);
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

            if (_currentDocumentObject == null)
                return;

            if (eventArgs.Path.Matches("format", "dokumentobjekt"))
            {
                string format = eventArgs.Value.ToLower();
                
                if (_numberOfUniqueDocumentFormats.ContainsKey(format))
                    _numberOfUniqueDocumentFormats[format]++;
                else
                    _numberOfUniqueDocumentFormats.Add(format, 1);

                _currentDocumentObject.Format = format;
            }

            if (eventArgs.Path.Matches("referanseDokumentfil", "dokumentobjekt"))
                _currentDocumentObject.FileReference = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentobjekt") && _currentDocumentObject != null)
            {
                if (_documentObjectsPerArchivePart.ContainsKey(_currentArchivePart))
                    _documentObjectsPerArchivePart[_currentArchivePart].Add(_currentDocumentObject);
                else
                    _documentObjectsPerArchivePart.Add(_currentArchivePart, new List<DocumentObject>
                    {
                        _currentDocumentObject
                    });

                _currentDocumentObject = null;
            }

            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        private class DocumentObject
        {
            public string Format { get; set; }
            public string FileReference { get; set; }
            public int XmlLineNumber { get; set; }

            public DocumentObject(int xmlLineNumber)
            {
                XmlLineNumber = xmlLineNumber;
            }

            public bool HasFormatMismatch()
            {
                var fileExtension = Path.GetExtension(FileReference)?.ToLower();
                return Format != null && fileExtension != null && !fileExtension.Equals($".{Format}");
            }
        }
    }
}
