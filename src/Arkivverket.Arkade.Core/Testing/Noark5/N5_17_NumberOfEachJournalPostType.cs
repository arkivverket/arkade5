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

        private readonly Dictionary<ArchivePart, Dictionary<string, JournalPostType>> _journalPostTypesPerArchivePart =
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
            bool multipleArchiveParts = _journalPostTypesPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();

            if (_journalPostTypesPerArchivePart.Count == 0)
            {
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.TotalResultNumber, 0)));
                return testResultSet;
            }

            foreach ((ArchivePart archivePart, Dictionary<string, JournalPostType> numberOfJournalPostTypes) in
                _journalPostTypesPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((string journalPostTypeName, JournalPostType journalPostType) in numberOfJournalPostTypes)
                {
                    ResultType resultType = journalPostTypeName.Equals(string.Empty) 
                        ? ResultType.Error 
                        : ResultType.Success;

                    Location testResultLocation = resultType == ResultType.Success
                        ? new Location(string.Empty)
                        : new Location(ArkadeConstants.ArkivuttrekkXmlFileName, journalPostType.Locations);

                    testResults.Add(new TestResult(resultType, testResultLocation, string.Format(
                        Noark5Messages.NumberOfEachJournalPostTypeMessage_TypeAndCount, journalPostTypeName, journalPostType.Count)));
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
                long xmlLineNumber = eventArgs.LineNumber;
                if (_journalPostTypesPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_journalPostTypesPerArchivePart[_currentArchivePart].ContainsKey(journalPostType))
                    {
                        _journalPostTypesPerArchivePart[_currentArchivePart][journalPostType].Count++;
                        _journalPostTypesPerArchivePart[_currentArchivePart][journalPostType].Locations.Add(xmlLineNumber);
                    }
                    else
                        _journalPostTypesPerArchivePart[_currentArchivePart].Add(journalPostType, new JournalPostType(xmlLineNumber));
                }
                else
                {
                    _journalPostTypesPerArchivePart.Add(_currentArchivePart, new Dictionary<string, JournalPostType>
                    {
                        {journalPostType, new JournalPostType(xmlLineNumber)}
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


        private class JournalPostType
        {
            public int Count { get; set; }
            public List<long> Locations { get; }

            public JournalPostType(long xmlLineNumber)
            {
                Count = 1;
                Locations = new List<long> { xmlLineNumber };
            }
        }
    }
}
