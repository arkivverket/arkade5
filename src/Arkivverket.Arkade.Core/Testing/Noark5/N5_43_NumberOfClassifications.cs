using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_43_NumberOfClassifications : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 43);

        private readonly Dictionary<ArchivePart, Dictionary<string, int>>
            _numberOfClassificationsPerElementPerArchivePart = new();
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
            bool multipleArchiveParts = _numberOfClassificationsPerElementPerArchivePart.Count > 1;

            int totalNumberOfClassifications =
                _numberOfClassificationsPerElementPerArchivePart.Sum(a => a.Value.Sum(c => c.Value));

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfClassifications))
                }
            };

            if (totalNumberOfClassifications == 0)
                return testResultSet;

            foreach ((ArchivePart archivePart, Dictionary<string, int> classificationsPerElement) in
                _numberOfClassificationsPerElementPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((string parentElementName, int numberOfClassifications) in classificationsPerElement)
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                        string.Format(Noark5Messages.NumberOfClassificationsMessage, parentElementName,
                            numberOfClassifications)));

                if (multipleArchiveParts)
                {
                    testResults.Insert(0, new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOf, classificationsPerElement.Values.Sum())));

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
            if (eventArgs.NameEquals("gradering") && eventArgs.Path.GetParent() != "gradering")
            {
                string parentElementName = eventArgs.Path.GetParent();

                if (_numberOfClassificationsPerElementPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_numberOfClassificationsPerElementPerArchivePart[_currentArchivePart].ContainsKey(parentElementName))
                        _numberOfClassificationsPerElementPerArchivePart[_currentArchivePart][parentElementName]++;
                    else
                        _numberOfClassificationsPerElementPerArchivePart[_currentArchivePart].Add(parentElementName, 1);
                }
                else
                {
                    _numberOfClassificationsPerElementPerArchivePart.Add(
                        _currentArchivePart, new Dictionary<string, int>
                        {
                            {parentElementName, 1}
                        }
                    );
                }
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }
    }
}
