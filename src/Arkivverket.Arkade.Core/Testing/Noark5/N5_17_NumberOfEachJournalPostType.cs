using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_17_NumberOfEachJournalPostType : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 17);

        private readonly Dictionary<ArchivePart, Dictionary<string, int>> _numberOfJournalPostTypesPerArchivePart =
            new();
        private ArchivePart _currentArchivePart = new();
        private string _currentJournalPostSystemId;
        private bool _journalPostAttributeIsFound;

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
            bool multipleArchiveParts = _numberOfJournalPostTypesPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();

            if (_numberOfJournalPostTypesPerArchivePart.Count == 0)
            {
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.TotalResultNumber, 0)));
                return testResultSet;
            }

            foreach ((ArchivePart archivePart, Dictionary<string, int> numberOfJournalPostTypes) in
                _numberOfJournalPostTypesPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((string journalPostType, int count) in numberOfJournalPostTypes)
                {
                    ResultType resultType = journalPostType.Equals(string.Empty) 
                        ? ResultType.Error 
                        : ResultType.Success;

                    testResults.Add(new TestResult(resultType, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfEachJournalPostTypeMessage_TypeAndCount, journalPostType, count)));
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
            if (Noark5TestHelper.IdentifiesJournalPostRegistration(eventArgs))
                _journalPostAttributeIsFound = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("systemID", "registrering") && _journalPostAttributeIsFound)
                _currentJournalPostSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("journalposttype", "registrering") && _journalPostAttributeIsFound)
            {
                string journalPostType = eventArgs.Value;
                if (_numberOfJournalPostTypesPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_numberOfJournalPostTypesPerArchivePart[_currentArchivePart].ContainsKey(journalPostType))
                        _numberOfJournalPostTypesPerArchivePart[_currentArchivePart][journalPostType]++;
                    else
                        _numberOfJournalPostTypesPerArchivePart[_currentArchivePart].Add(journalPostType, 1);
                }
                else
                {
                    _numberOfJournalPostTypesPerArchivePart.Add(_currentArchivePart, new Dictionary<string, int>
                    {
                        {journalPostType, 1}
                    });
                }
            }

        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new ArchivePart();
            }

            if (!eventArgs.NameEquals("registrering"))
                return;

            _journalPostAttributeIsFound = false; // reset
            _currentJournalPostSystemId = ""; // reset
        }
    }
}
