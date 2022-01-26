using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_34_NumberOfMultiReferencedDocumentFiles : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 34);

        private readonly Dictionary<ArchivePart, Dictionary<string, DocumentFileReference>>
            _numberOfReferencesPerDocumentFilePerArchivePart = new();
        private ArchivePart _currentArchivePart = new();

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
            bool multipleArchiveParts = _numberOfReferencesPerDocumentFilePerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();

            var totalNumberOfMultiReferencedDocumentFiles = 0;

            foreach ((ArchivePart archivePart, Dictionary<string, DocumentFileReference> referencesPerDocumentFile) in
                _numberOfReferencesPerDocumentFilePerArchivePart)
            {
                var testResults = new List<TestResult>();

                var numberOfMultiReferencedDocumentFiles = 0;

                foreach ((string reference, DocumentFileReference documentFileReference) in referencesPerDocumentFile.Where(r => r.Value.Count > 1))
                {
                    testResults.Add(new TestResult(ResultType.Error, 
                        new Location(ArkadeConstants.ArkivuttrekkXmlFileName, documentFileReference.Locations),
                        string.Format(Noark5Messages.NumberOfMultiReferencedDocumentFilesMessage, reference, documentFileReference.Count)));
                    numberOfMultiReferencedDocumentFiles++;
                }

                if (numberOfMultiReferencedDocumentFiles > 0)
                {
                    if (multipleArchiveParts)
                    {
                        testResults.Insert(0, new TestResult(ResultType.Error, new Location(string.Empty), 
                            string.Format(Noark5Messages.NumberOf, numberOfMultiReferencedDocumentFiles)));
                        testResultSet.TestResultSets.Add(new TestResultSet
                        {
                            Name = archivePart.ToString(),
                            TestsResults = testResults,
                        });
                    }
                    else
                    {
                        testResultSet.TestsResults = testResults;
                    }
                }

                totalNumberOfMultiReferencedDocumentFiles += numberOfMultiReferencedDocumentFiles;
            }

            ResultType resultType = totalNumberOfMultiReferencedDocumentFiles == 0
                ? ResultType.Success
                : ResultType.Error;

            testResultSet.TestsResults.Insert(0, new TestResult(resultType, new Location(string.Empty),
                string.Format(Noark5Messages.TotalResultNumber, totalNumberOfMultiReferencedDocumentFiles)));

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
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

            if (eventArgs.Path.Matches("referanseDokumentfil", "dokumentobjekt"))
            {
                string reference = eventArgs.Value;
                long xmlLineNumber = eventArgs.LineNumber;
                if (_numberOfReferencesPerDocumentFilePerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_numberOfReferencesPerDocumentFilePerArchivePart[_currentArchivePart].ContainsKey(reference))
                        _numberOfReferencesPerDocumentFilePerArchivePart[_currentArchivePart][reference].Count++;
                    else
                        _numberOfReferencesPerDocumentFilePerArchivePart[_currentArchivePart].Add(reference, new DocumentFileReference(xmlLineNumber));
                }
                else
                {
                    _numberOfReferencesPerDocumentFilePerArchivePart.Add(_currentArchivePart, new Dictionary<string, DocumentFileReference>
                    {
                        {reference, new DocumentFileReference(xmlLineNumber)}
                    });
                }
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        private class DocumentFileReference
        {
            public int Count { get; set; }
            public List<long> Locations { get; }

            public DocumentFileReference(long xmlLineNumber)
            {
                Count = 1;
                Locations = new List<long> { xmlLineNumber };
            }
        }
    }
}
