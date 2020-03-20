using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_21_NumberOfRegistrationsWithoutDocumentDescription : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 21);

        private bool _documentDescriptionIsFound;
        private readonly Dictionary<ArchivePart, int> _noDocumentDescriptionCountPerArchivepart = new Dictionary<ArchivePart, int>();
        private ArchivePart _currentArchivePart = new ArchivePart();
        private int _totalNumberOfMissingDocumentDescriptions;

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(string.Empty), string.Format(Noark5Messages.TotalResultNumber, 
                    _totalNumberOfMissingDocumentDescriptions.ToString()))
            };

            if (_noDocumentDescriptionCountPerArchivepart.Count > 1)
            {
                foreach (KeyValuePair<ArchivePart, int> noDocumentDescriptionCount in _noDocumentDescriptionCountPerArchivepart)
                {
                    if (noDocumentDescriptionCount.Value > 0)
                    {
                        var testResult = new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.NumberOf_PerArchivePart,
                            noDocumentDescriptionCount.Key.SystemId, noDocumentDescriptionCount.Key.Name, noDocumentDescriptionCount.Value));

                        testResults.Add(testResult);
                    }
                }
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentbeskrivelse"))
                _documentDescriptionIsFound = true;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                _noDocumentDescriptionCountPerArchivepart.Add(_currentArchivePart, 0);
            }
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
            {
                _currentArchivePart.Name = eventArgs.Value;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering"))
            {
                if (!_documentDescriptionIsFound)
                {
                    _totalNumberOfMissingDocumentDescriptions++;

                    if (_noDocumentDescriptionCountPerArchivepart.ContainsKey(_currentArchivePart))
                        _noDocumentDescriptionCountPerArchivepart[_currentArchivePart]++;
                }
                _documentDescriptionIsFound = false;
            }

            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

    }
}
