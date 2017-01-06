using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class FirstAndLastRegistrationCreationDates : Noark5XmlReaderBaseTest
    {
        private readonly SortedSet<DateTime> _registrationCreationDates = new SortedSet<DateTime>();
        private int _invalidRegistrationCreationDateCount;
        private int _registrationCount;

        public override string GetName()
        {
            return Noark5Messages.DatesFirstAndLastRegistration;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            testResults.Add(new TestResult(ResultType.Success, new Location(""),
                string.Format(Noark5Messages.DatesFirstAndLastRegistrationMessage_NumberOfRegistrations, _registrationCount)));

            if (_registrationCreationDates.Any())
            {
                testResults.Add(new TestResult(ResultType.Success, new Location(""),
                    string.Format(Noark5Messages.DatesFirstAndLastRegistrationMessage_CreationDateFirstRegistration, _registrationCreationDates.First().ToShortDateString())));
                testResults.Add(new TestResult(ResultType.Success, new Location(""),
                    string.Format(Noark5Messages.DatesFirstAndLastRegistrationMessage_CreationDateLastRegistration, _registrationCreationDates.Last().ToShortDateString())));
            }
            else
                testResults.Add(new TestResult(ResultType.Error, new Location(""),
                    Noark5Messages.DatesFirstAndLastRegistrationMessage_NoValidRegistrationCreationDates));

            if (_invalidRegistrationCreationDateCount > 0)
                testResults.Add(new TestResult(ResultType.Error, new Location(""),
                    string.Format(Noark5Messages.DatesFirstAndLastRegistrationMessage_NumberOfInvalidRegistrationCreationDates, _invalidRegistrationCreationDateCount)));

            return testResults;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (!eventArgs.Path.Matches("opprettetDato", "registrering"))
                return;

            DateTime registrationCreatedTime;

            if (DateTime.TryParse(eventArgs.Value, out registrationCreatedTime))
                _registrationCreationDates.Add(registrationCreatedTime);

            else
                _invalidRegistrationCreationDateCount++;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("opprettetDato", "registrering"))
                _registrationCount++;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
