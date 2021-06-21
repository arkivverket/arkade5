using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_06_StatusOfArchiveParts : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 6);

        private readonly Dictionary<ArchivePart, string> _statusPerArchivePart = new();
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
            var testResultSet = new TestResultSet();

            bool multipleArchiveParts = _statusPerArchivePart.Count > 1;

            foreach ((ArchivePart archivePart, string status) in _statusPerArchivePart)
            {
                string resultMessage = multipleArchiveParts
                    ? string.Format(Noark5Messages.StatusOfArchivePartsMessage_ForArchivePart, archivePart, status)
                    : string.Format(Noark5Messages.StatusOfArchivePartsMessage, status);

                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(resultMessage)));
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
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("arkivdelstatus", "arkivdel"))
                _statusPerArchivePart.Add(_currentArchivePart, eventArgs.Value);
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }
    }
}
