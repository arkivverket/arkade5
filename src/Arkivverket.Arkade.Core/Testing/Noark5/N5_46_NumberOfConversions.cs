using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_46_NumberOfConversions : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 46);

        private ArchivePart _currentArchivePart = new();
        private readonly Dictionary<ArchivePart, int> _numberOfConversionsPerArchivePart = new();

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
            bool multipleArchiveParts = _numberOfConversionsPerArchivePart.Count > 1;

            int totalNumberOfConversions = _numberOfConversionsPerArchivePart.Sum(a => a.Value);
            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfConversions))
                }
            };

            if (!multipleArchiveParts)
                return testResultSet;

            foreach ((ArchivePart archivePart, int numberOfConversions) in _numberOfConversionsPerArchivePart)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfXPerY, archivePart, numberOfConversions)));

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("konvertering", "dokumentobjekt"))
                _numberOfConversionsPerArchivePart[_numberOfConversionsPerArchivePart.Keys.Last()]++;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                _numberOfConversionsPerArchivePart.Add(_currentArchivePart, 0);
            }
                
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

    }
}
