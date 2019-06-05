using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class NumberOfRegistrations : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 16);
        private int _journalPostRegistrationCount;
        private int _meetingRegistrationCount;
        private int _totalRegistrationsCount;

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
            return new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.TotalRegistrationCount, _totalRegistrationsCount)),

                new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.JournalPostRegistrationCount, _journalPostRegistrationCount)),

                new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.MeetingRegistrationCount, _meetingRegistrationCount))
            };
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering"))
                _totalRegistrationsCount++;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesJournalPostRegistration(eventArgs))
                _journalPostRegistrationCount++;

            if (Noark5TestHelper.IdentifiesMeetingRegistration(eventArgs))
                _meetingRegistrationCount++;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
