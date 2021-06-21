using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_21_NumberOfRegistrationsWithoutDocumentDescription : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 21);

        private bool _documentDescriptionIsFound;
        private readonly Dictionary<ArchivePart, int> _noDocumentDescriptionCountPerArchivePart = new();
        private ArchivePart _currentArchivePart = new();
        private int _totalNumberOfMissingDocumentDescriptions;

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
            bool multipleArchiveParts = _noDocumentDescriptionCountPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(Noark5Messages.TotalResultNumber,
                        _totalNumberOfMissingDocumentDescriptions))
                }
            };

            if (!multipleArchiveParts || _totalNumberOfMissingDocumentDescriptions == 0)
                return testResultSet;

            foreach ((ArchivePart archivePart, int noDocumentDescriptionCount) in
                _noDocumentDescriptionCountPerArchivePart)
            {
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfXPerY, archivePart, noDocumentDescriptionCount)));
            }

            return testResultSet;
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
                _currentArchivePart.SystemId = eventArgs.Value;
            
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
            
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering"))
            {
                if (!_documentDescriptionIsFound)
                {
                    _totalNumberOfMissingDocumentDescriptions++;

                    if (_noDocumentDescriptionCountPerArchivePart.ContainsKey(_currentArchivePart))
                        _noDocumentDescriptionCountPerArchivePart[_currentArchivePart]++;
                    else
                        _noDocumentDescriptionCountPerArchivePart.Add(_currentArchivePart, 1);
                }
                _documentDescriptionIsFound = false;
            }

            if(eventArgs.NameEquals("arkivdel"))
            {
                if (!_noDocumentDescriptionCountPerArchivePart.ContainsKey(_currentArchivePart))
                    _noDocumentDescriptionCountPerArchivePart.Add(_currentArchivePart, 0);

                _currentArchivePart = new ArchivePart();
            }
        }

    }
}
