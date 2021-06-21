using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_41_NumberOfDocumentFlows : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 41);

        private int _totalNumberOfDocumentFlows;
        private ArchivePart _currentArchivePart = new();
        private bool _journalPostAttributeIsFound;
        private readonly Dictionary<ArchivePart, int> _documentFlowsPerArchivePart = new();

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
            bool multipleArchiveParts = _documentFlowsPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _totalNumberOfDocumentFlows))
                }
            };

            if (_totalNumberOfDocumentFlows == 0 || !multipleArchiveParts)
                return testResultSet;

            foreach ((ArchivePart archivePart, int correspondencePartCount) in _documentFlowsPerArchivePart)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfXPerY, archivePart, correspondencePartCount)));

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesJournalPostRegistration(eventArgs))
                _journalPostAttributeIsFound = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                _documentFlowsPerArchivePart.Add(_currentArchivePart, 0);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentflyt"))
            {
                if (_journalPostAttributeIsFound)
                {
                    _totalNumberOfDocumentFlows++;

                    if (_documentFlowsPerArchivePart.Count > 0)
                    {
                        if (_documentFlowsPerArchivePart.ContainsKey(_currentArchivePart))
                            _documentFlowsPerArchivePart[_currentArchivePart]++;
                    }
                }

                _journalPostAttributeIsFound = false;
            }

            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

    }
}
