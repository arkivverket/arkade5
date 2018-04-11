using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <inheritdoc />
    public class NumberOfDepreciations : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 40);

        private int _totalNumberOfDeprecations;
        private readonly Dictionary<string, int> _numberOfDeprecationsPerArchivePart = new Dictionary<string, int>();
        private string _currentArchivePartSystemId;

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfDepreciations;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>()
            {
                new TestResult(ResultType.Success, new Location(string.Empty), string.Format(Noark5Messages.NumberOfDepreciationsMessage, _totalNumberOfDeprecations))
            };

            if (_numberOfDeprecationsPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> numberOfDepreciation in _numberOfDeprecationsPerArchivePart)
                {
                    if (numberOfDepreciation.Value > 0)
                    {
                        var testresult = new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOfDepreciationsMessage_ForArchivePart, numberOfDepreciation.Key,
                                numberOfDepreciation.Value));

                        testResults.Add(testresult);
                    }
                }
            }
            return testResults;
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
                _currentArchivePartSystemId = eventArgs.Value;
                _numberOfDeprecationsPerArchivePart.Add(_currentArchivePartSystemId, 0);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("referanseAvskrivesAvJournalpost"))
            {
                _totalNumberOfDeprecations++;

                if (_numberOfDeprecationsPerArchivePart.Count > 0)
                {
                    if (_numberOfDeprecationsPerArchivePart.ContainsKey(_currentArchivePartSystemId))
                        _numberOfDeprecationsPerArchivePart[_currentArchivePartSystemId]++;
                }
            }
        }
    }
}
