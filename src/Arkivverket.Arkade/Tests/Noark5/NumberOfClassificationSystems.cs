using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfClassificationSystems : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 7);

        private readonly Dictionary<string, int> _classificationSystemsPerArchivePart = new Dictionary<string, int>();
        private string _currentArchivePartSystemId = string.Empty;

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfClassificationSystems;
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
                    Noark5Messages.NumberOfClassificationSystemsMessage,
                    _classificationSystemsPerArchivePart.Values.Sum()))
            };

            if (_classificationSystemsPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> classificationCountAtLevel in _classificationSystemsPerArchivePart)
                {
                    var testResult = new TestResult(ResultType.Success, new Location(string.Empty),
                        string.Format(
                            Noark5Messages.NumberOfClassificationSystemsMessage_ClassificationSystemInArchivePart,
                            classificationCountAtLevel.Key, classificationCountAtLevel.Value));

                    testResults.Add(testResult);
                }
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("klassifikasjonssystem", "arkivdel"))
            {
                if (_classificationSystemsPerArchivePart.ContainsKey(_currentArchivePartSystemId))
                    _classificationSystemsPerArchivePart[_currentArchivePartSystemId]++;
                else
                    _classificationSystemsPerArchivePart.Add(_currentArchivePartSystemId, 1);
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePartSystemId = string.Empty;
        }
    }
}