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

        private readonly Dictionary<ArchivePart, Dictionary<string, JournalPostStatus>> _journalPostStatusesPerArchivePart = new();
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
            bool multipleArchiveParts = _journalPostStatusesPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();

            foreach ((ArchivePart archivePart, Dictionary<string, JournalPostStatus> journalPostStatuses) in _journalPostStatusesPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((string status, JournalPostStatus journalPostStatus) in journalPostStatuses)
                {
                    ResultType resultType = status.Equals("Arkivert") || status.Equals("Utgår")
                        ? ResultType.Success
                        : ResultType.Error;

                    Location testResultLocation = resultType == ResultType.Success
                        ? new Location(string.Empty)
                        : new Location(ArkadeConstants.ArkivuttrekkXmlFileName, journalPostStatus.Locations);

                    testResults.Add(new TestResult(resultType, testResultLocation,
                        string.Format(Noark5Messages.NumberOfEachJournalStatusMessage, status, journalPostStatus.Count)
                    ));
                }

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
                string journalPostStatus = eventArgs.Value;
                long xmlLineNumber = eventArgs.LineNumber;

                if (_journalPostStatusesPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_journalPostStatusesPerArchivePart[_currentArchivePart].ContainsKey(journalPostStatus))
                    {
                        _journalPostStatusesPerArchivePart[_currentArchivePart][journalPostStatus].Count++;
                        _journalPostStatusesPerArchivePart[_currentArchivePart][journalPostStatus].Locations.Add(xmlLineNumber);
                    }
                    else
                        _journalPostStatusesPerArchivePart[_currentArchivePart].Add(journalPostStatus, new JournalPostStatus(xmlLineNumber));
                }
                else
                {
                    _journalPostStatusesPerArchivePart.Add(_currentArchivePart, new Dictionary<string, JournalPostStatus>
                    {
                        {journalPostStatus, new JournalPostStatus(xmlLineNumber)}
                    });
                }
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if(eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new ArchivePart();
            }
        }

        private class JournalPostStatus
        {
            public int Count { get; set; }
            public List<long> Locations { get; }

            public JournalPostStatus(long xmlLineNumber)
            {
                Count = 1;
                Locations = new List<long> { xmlLineNumber };
            }
        }
    }
}
