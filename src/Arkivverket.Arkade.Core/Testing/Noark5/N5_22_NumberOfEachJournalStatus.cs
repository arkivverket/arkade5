using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_22_NumberOfEachJournalStatus : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 22);

        private readonly List<N5_22_ArchivePart> _archiveParts = new();
        private N5_22_ArchivePart _currentArchivePart = new();

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
            bool multipleArchiveParts = _archiveParts.Count > 1;

            var testResultSet = new TestResultSet();

            foreach (N5_22_ArchivePart archivePart in _archiveParts)
            {
                var testResults = new List<TestResult>();

                foreach ((string status, int count) in archivePart.NumberOfJournalPostStatuses)
                    testResults.Add(new TestResult(
                        status.Equals("Arkivert") || status.Equals("Utgår")
                            ? ResultType.Success
                            : ResultType.Error,
                        new Location(string.Empty),
                        string.Format(Noark5Messages.NumberOfEachJournalStatusMessage, status, count)
                    ));

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                else
                    testResultSet.TestsResults = testResults;
            }

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

            if (eventArgs.Path.Matches("journalstatus", "registrering"))
            {
                string status = eventArgs.Value;

                if (_currentArchivePart.NumberOfJournalPostStatuses.ContainsKey(status))
                    _currentArchivePart.NumberOfJournalPostStatuses[status]++;
                else
                    _currentArchivePart.NumberOfJournalPostStatuses.Add(status, 1);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if(eventArgs.NameEquals("arkivdel"))
            {
                _archiveParts.Add(_currentArchivePart);
                _currentArchivePart = new N5_22_ArchivePart();
            }
        }

        private class N5_22_ArchivePart : ArchivePart
        {
            public readonly Dictionary<string, int> NumberOfJournalPostStatuses = new();
        }
    }
}
