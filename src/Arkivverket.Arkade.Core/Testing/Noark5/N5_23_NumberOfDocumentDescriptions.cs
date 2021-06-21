using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_23_NumberOfDocumentDescriptions : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 23);

        private readonly Dictionary<ArchivePart, int> _documentDescriptionsPerArchivePart = new();
        private ArchivePart _currentArchivePart = new();
        private int _totalNumberOfDocumentDescriptions;

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
            bool multipleArchiveParts = _documentDescriptionsPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _totalNumberOfDocumentDescriptions))
                }
            };

            if (!multipleArchiveParts)
                return testResultSet;

            foreach ((ArchivePart archivePart, int documentDescriptionCount) in _documentDescriptionsPerArchivePart)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfXPerY, archivePart, documentDescriptionCount)));

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
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                _documentDescriptionsPerArchivePart.Add(_currentArchivePart, 0);
            }
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
            {
                _currentArchivePart.Name = eventArgs.Value;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentbeskrivelse"))
            {
                _totalNumberOfDocumentDescriptions++;

                if (_documentDescriptionsPerArchivePart.Count > 0)
                {
                    if (_documentDescriptionsPerArchivePart.ContainsKey(_currentArchivePart))
                        _documentDescriptionsPerArchivePart[_currentArchivePart]++;
                }
              
            }
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

    }
}