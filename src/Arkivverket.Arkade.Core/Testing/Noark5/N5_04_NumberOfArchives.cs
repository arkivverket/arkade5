using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_04_NumberOfArchives : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 4);

        private readonly Dictionary<int, int> _archiveCountByLevel = new Dictionary<int, int>();
        private int _level = 1;

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
            var testResultSet = new List<TestResult>
            {
                new(ResultType.Success, new Location(string.Empty), string.Format(
                    Noark5Messages.TotalResultNumber, _archiveCountByLevel.Values.Sum()
                ))
            };

            if (_archiveCountByLevel.Count > 1)
            {
                foreach ((int level, int numberOfArchives) in _archiveCountByLevel)
                {
                    var testResult = new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfArchivesMessage_ArchivesAtLevel, level, numberOfArchives));

                    testResultSet.Add(testResult);
                }
            }

            return new TestResultSet()
            {
                TestsResults = testResultSet
            };
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
