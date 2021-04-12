using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_27_FirstAndLastRegistrationCreationDates : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 27);

        private int _invalidRegistrationCreationDateCount;
        private int _registrationCount;
        private N5_27_ArchivePart _currentArchivePart;
        private readonly List<N5_27_ArchivePart> _archiveParts = new List<N5_27_ArchivePart>();

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

            testResults.Add(new TestResult(ResultType.Success, new Location(""),
                string.Format(Noark5Messages.DatesFirstAndLastRegistrationMessage_NumberOfRegistrations,
                    _registrationCount)));

            foreach (N5_27_ArchivePart archivePart in _archiveParts)
            {
                if (archivePart.RegistrationCreationDates.Any())
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(""),
                        string.Format(
                            Noark5Messages
                                .DatesFirstAndLastRegistrationMessage_CreationDateFirstRegistration_InArchivePart,
                            archivePart.SystemId, archivePart.Name,
                            archivePart.RegistrationCreationDates.First().ToString("dd.MM.yyyy"))));
                    testResults.Add(new TestResult(ResultType.Success, new Location(""),
                        string.Format(
                            Noark5Messages
                                .DatesFirstAndLastRegistrationMessage_CreationDateLastRegistration_InArchivePart,
                            archivePart.SystemId, archivePart.Name,
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
                _currentArchivePart = new N5_27_ArchivePart {SystemId = eventArgs.Value};
                _archiveParts.Add(_currentArchivePart);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (!eventArgs.Path.Matches("opprettetDato", "registrering"))
                return;

            if (Noark5TestHelper.TryParseValidXmlDate(eventArgs.Value, out DateTime registrationCreatedTime))
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
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new N5_27_ArchivePart();
        }

        private class N5_27_ArchivePart : ArchivePart
        {
            public readonly SortedSet<DateTime> RegistrationCreationDates = new SortedSet<DateTime>();
        }
    }
}
