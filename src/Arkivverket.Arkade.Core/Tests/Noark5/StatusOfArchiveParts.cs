using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Tests.Noark5
{
    public class StatusOfArchiveParts : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 6);

        private readonly List<ArchivePart> _archiveParts = new List<ArchivePart>();
        private ArchivePart _currentArchivePart;

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.StatusOfArchiveParts;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            if (_archiveParts.Count == 1)
            {
                string archivePartStatus = _archiveParts.FirstOrDefault()?.Status;

                testResults.Add(new TestResult(ResultType.Success, new Location(""),
                    string.Format(Noark5Messages.StatusOfArchivePartsMessage, archivePartStatus)));
            }
            else
            {
                foreach (ArchivePart archivePart in _archiveParts)
                {
                    testResults.Add(
                        new TestResult(ResultType.Success, new Location(""), string.Format(
                            Noark5Messages.StatusOfArchivePartsMessage_ForArchivePart,
                            archivePart.SystemId,
                            archivePart.Status
                        )));
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
                _currentArchivePart = new ArchivePart {SystemId = eventArgs.Value};

            if (eventArgs.Path.Matches("arkivdelstatus", "arkivdel"))
                _currentArchivePart.Status = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
                _archiveParts.Add(_currentArchivePart);
        }

        private class ArchivePart
        {
            public string SystemId;
            public string Status;
        }
    }
}
