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

        private readonly Dictionary<ArchivePart, Dictionary<string, int>>
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

            foreach ((ArchivePart archivePart, Dictionary<string, int> referencesPerDocumentFile) in
                _numberOfReferencesPerDocumentFilePerArchivePart)
            {
                var testResults = new List<TestResult>();

                var numberOfMultiReferencedDocumentFiles = 0;

                foreach ((string reference, int count) in referencesPerDocumentFile.Where(r => r.Value > 1))
                {
                    testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfMultiReferencedDocumentFilesMessage, reference, count)));
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
                if (_numberOfReferencesPerDocumentFilePerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_numberOfReferencesPerDocumentFilePerArchivePart[_currentArchivePart].ContainsKey(reference))
                        _numberOfReferencesPerDocumentFilePerArchivePart[_currentArchivePart][reference]++;
                    else
                        _numberOfReferencesPerDocumentFilePerArchivePart[_currentArchivePart].Add(reference, 1);
                }
                else
                {
                    _numberOfReferencesPerDocumentFilePerArchivePart.Add(_currentArchivePart, new Dictionary<string, int>
                    {
                        {reference, 1}
                    });
                }
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }
    }
}
