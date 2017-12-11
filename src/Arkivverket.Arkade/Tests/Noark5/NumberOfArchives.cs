using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfArchives : Noark5XmlReaderBaseTest
    {
        private readonly Dictionary<int, int> _archiveCountByLevel = new Dictionary<int, int>();
        private int _level = 1;

        public override string GetName()
        {
            return Noark5Messages.NumberOfArchives;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                    Noark5Messages.NumberOfArchivesMessage, _archiveCountByLevel.Values.Sum()
                ))
            };

            if (_archiveCountByLevel.Count > 1)
            {
                foreach (KeyValuePair<int, int> archivesCountAtLevel in _archiveCountByLevel)
                {
                    var testResult = new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfArchivesMessage_ArchivesAtLevel,
                        archivesCountAtLevel.Key, archivesCountAtLevel.Value));

                    testResults.Add(testResult);
                }
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkiv"))
                CountArchiveAndBumpLevel();
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkiv"))
                _level--;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private void CountArchiveAndBumpLevel()
        {
            if (_archiveCountByLevel.ContainsKey(_level))
                _archiveCountByLevel[_level]++;

            else _archiveCountByLevel.Add(_level, 1);

            _level++;
        }
    }
}
