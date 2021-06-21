using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    /// <inheritdoc />
    public class N5_40_NumberOfDepreciations : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 40);

        private int _totalNumberOfDeprecations;
        private readonly List<N5_40_ArchivePart> _archiveParts = new List<N5_40_ArchivePart>();
        private N5_40_ArchivePart _currentArchivePart;

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

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(""), string.Format(
                        Noark5Messages.TotalResultNumber, _totalNumberOfDeprecations))
                }
            };

            if (_totalNumberOfDeprecations == 0)
                return testResultSet;

            foreach (N5_40_ArchivePart archivePart in _archiveParts)
            {
                var testResults = new List<TestResult>();

                foreach ((string type, int numberOf) in archivePart.TypeOfDepreciation)
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                        string.Format(Noark5Messages.NumberOfXPerY, type, numberOf)));

                if (multipleArchiveParts)
                {
                    testResults.Insert(0, new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOf, archivePart.TypeOfDepreciation.Values.Sum())));

                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                }
                else
                    testResultSet.TestsResults.AddRange(testResults);
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
            {
                _currentArchivePart = new N5_40_ArchivePart { SystemId = eventArgs.Value };
                _archiveParts.Add(_currentArchivePart);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("avskrivningsmaate", "avskrivning"))
            {
                _totalNumberOfDeprecations++;

                string type = eventArgs.Value;

                if (_currentArchivePart.TypeOfDepreciation.ContainsKey(type))
                {
                    _currentArchivePart.TypeOfDepreciation[type]++;
                }
                else
                {
                    _currentArchivePart.TypeOfDepreciation.Add(type, 1);
                }
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class N5_40_ArchivePart : ArchivePart
        {
            public readonly Dictionary<string, int> TypeOfDepreciation = new Dictionary<string, int>();
        }
    }
}
