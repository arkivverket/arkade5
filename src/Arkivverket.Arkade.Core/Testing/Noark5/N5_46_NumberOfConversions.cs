using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_46_NumberOfConversions : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 46);
        private ArchivePart _currentArchivePart = new ArchivePart();
        private readonly Dictionary<ArchivePart, int> _numberOfConvertionsPerArchivePart = new Dictionary<ArchivePart, int>();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();
            int totalNumberOfConversions = 0;

            foreach (var archivePartConvertionsCount in _numberOfConvertionsPerArchivePart)
            {
                if (archivePartConvertionsCount.Value == 0)
                    continue;

                totalNumberOfConversions += archivePartConvertionsCount.Value;

                if (_numberOfConvertionsPerArchivePart.Keys.Count > 1) // Multiple archiveparts
                {
                    testResults.Insert(0, new TestResult(ResultType.Success, new Location(""),
                        string.Format(Noark5Messages.ArchivePartSystemId, archivePartConvertionsCount.Key.SystemId, archivePartConvertionsCount.Key.Name) +
                        " - " + string.Format(Noark5Messages.TotalResultNumber, archivePartConvertionsCount.Value)));
                }
            }

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""),
                    string.Format(Noark5Messages.TotalResultNumber, totalNumberOfConversions)));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("konvertering", "dokumentobjekt"))
                _numberOfConvertionsPerArchivePart[_numberOfConvertionsPerArchivePart.Keys.Last()]++;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                _numberOfConvertionsPerArchivePart.Add(_currentArchivePart, 0);
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
