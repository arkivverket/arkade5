using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_07_NumberOfClassificationSystems : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 7);

        private readonly Dictionary<ArchivePart, int> _classificationSystemsPerArchivePart = new();
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
            int totalNumberOfClassificationSystems = _classificationSystemsPerArchivePart.Values.Sum();

            bool multipleArchiveParts = _classificationSystemsPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfClassificationSystems))
                }
            };

            if (!multipleArchiveParts)
                return testResultSet;

            foreach ((ArchivePart archivePart, int classificationSystemsCount) in _classificationSystemsPerArchivePart)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(string.Format(
                        Noark5Messages.NumberOfClassificationSystemsMessage_ClassificationSystemInArchivePart,
                        archivePart, classificationSystemsCount), classificationSystemsCount)));

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("klassifikasjonssystem", "arkivdel"))
            {
                if (_classificationSystemsPerArchivePart.ContainsKey(_currentArchivePart))
                    _classificationSystemsPerArchivePart[_currentArchivePart]++;
                else
                    _classificationSystemsPerArchivePart.Add(_currentArchivePart, 1);
            }
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
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }
       
 }
}