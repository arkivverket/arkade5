using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #3
    /// </summary>
    public class StatusOfArchiveParts : Noark5XmlReaderBaseTest
    {
        private readonly List<ArkivdelStatus> _arkivdelStatuses = new List<ArkivdelStatus>();

        private string _currentArkivdelName;
        private string _currentArkivdelStatus;

        public override string GetName()
        {
            return Noark5Messages.StatusOfArchiveParts;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
            {
                _arkivdelStatuses.Add(new ArkivdelStatus {Arkivdel = _currentArkivdelName, Status = _currentArkivdelStatus});

                _currentArkivdelName = null;
                _currentArkivdelStatus = null;
            }
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
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

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();
            foreach (ArkivdelStatus arkivdelStatus in _arkivdelStatuses)
            {
                testResults.Add(new TestResult(ResultType.Success, new Location(""), arkivdelStatus.Arkivdel + ": " + arkivdelStatus.Status));
            }
            return testResults;
        }

        private class ArkivdelStatus
        {
            public string Arkivdel;
            public string Status;
        }
    }
}