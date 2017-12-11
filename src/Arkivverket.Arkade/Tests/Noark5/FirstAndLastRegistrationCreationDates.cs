using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class FirstAndLastRegistrationCreationDates : Noark5XmlReaderBaseTest
    {
        private int _invalidRegistrationCreationDateCount;
        private int _registrationCount;
        private ArchivePart _currentArchivePart;
        private readonly List<ArchivePart> _archiveParts = new List<ArchivePart>();

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
                string.Format(Noark5Messages.DatesFirstAndLastRegistrationMessage_NumberOfRegistrations,
                    _registrationCount)));

            foreach (ArchivePart archivePart in _archiveParts)
            {
                if (archivePart.RegistrationCreationDates.Any())
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(""),
                        string.Format(
                            Noark5Messages
                                .DatesFirstAndLastRegistrationMessage_CreationDateFirstRegistration_InArchivePart,
                            archivePart.SystemId,
                            archivePart.RegistrationCreationDates.First().ToString("dd.MM.yyyy"))));
                    testResults.Add(new TestResult(ResultType.Success, new Location(""),
                        string.Format(
                            Noark5Messages
                                .DatesFirstAndLastRegistrationMessage_CreationDateLastRegistration_InArchivePart,
                            archivePart.SystemId,
                            archivePart.RegistrationCreationDates.Last().ToString("dd.MM.yyyy"))));
                }
            }


            if (_invalidRegistrationCreationDateCount > 0)
                testResults.Add(new TestResult(ResultType.Error, new Location(""),
                    string.Format(
                        Noark5Messages.DatesFirstAndLastRegistrationMessage_NumberOfInvalidRegistrationCreationDates,
                        _invalidRegistrationCreationDateCount)));

            return testResults;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart = new ArchivePart {SystemId = eventArgs.Value};
                _archiveParts.Add(_currentArchivePart);
            }

            if (!eventArgs.Path.Matches("opprettetDato", "registrering"))
                return;

            if (Noark5TestHelper.TryParseArchiveDate(eventArgs.Value, out DateTime registrationCreatedTime))
                _currentArchivePart.RegistrationCreationDates.Add(registrationCreatedTime);
            else
                _invalidRegistrationCreationDateCount++;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("opprettetDato", "registrering"))
                _registrationCount++;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class ArchivePart
        {
            public string SystemId { get; set; }
            public readonly SortedSet<DateTime> RegistrationCreationDates = new SortedSet<DateTime>();
        }
    }
}
