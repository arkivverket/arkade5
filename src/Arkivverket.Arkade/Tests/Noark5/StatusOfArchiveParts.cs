using System.Collections.Generic;
using System.Text;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class StatusOfArchiveParts : INoark5Test
    {
        private readonly List<ArkivdelStatus> _arkivdelStatuses = new List<ArkivdelStatus>();

        private string _currentArkivdelName;
        private string _currentArkivdelStatus;
        private TestRun _testRun;

        public StatusOfArchiveParts()
        {
            _testRun = new TestRun(GetName(), TestType.ContentAnalysis);
        }

        public string GetName()
        {
            return Noark5Messages.StatusOfArchiveParts;
        }

        public void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
        }

        public void OnReadEndElementEvent(object sender, ReadElementEventArgs e)
        {
            if (e.NameEquals("arkivdel"))
            {
                _arkivdelStatuses.Add(new ArkivdelStatus {Arkivdel = _currentArkivdelName, Status = _currentArkivdelStatus});

                _currentArkivdelName = null;
                _currentArkivdelStatus = null;
            }
        }

        public void OnReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
            {
                _currentArkivdelName = eventArgs.Value;
            }
            if (eventArgs.Path.Matches("arkivdelstatus", "arkivdel"))
            {
                _currentArkivdelStatus = eventArgs.Value;
            }
        }

        public TestRun GetTestRun()
        {
            foreach (ArkivdelStatus arkivdelStatus in _arkivdelStatuses)
            {
                _testRun.Add(new TestResult(ResultType.Success, new Location(""), arkivdelStatus.Arkivdel + ": " + arkivdelStatus.Status));
            }

            return _testRun;
        }

        private string CreateResultString()
        {
            var builder = new StringBuilder();

            foreach (ArkivdelStatus arkivdelStatus in _arkivdelStatuses)
            {
                builder.AppendLine(arkivdelStatus.Arkivdel + ": " + arkivdelStatus.Status);
            }

            return builder.ToString();
        }

        private class ArkivdelStatus
        {
            public string Arkivdel;
            public string Status;
        }
    }
}