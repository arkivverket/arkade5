using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_27_FirstAndLastRegistrationCreationDates : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 27);

        private int _invalidRegistrationCreationDateCount;
        private readonly List<long> _invalidRegistrationCreationDateLocations = new();
        private int _registrationCount;
        private N5_27_ArchivePart _currentArchivePart;
        private readonly List<N5_27_ArchivePart> _archiveParts = new();

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
            bool multipleArchiveParts = _archiveParts.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(""), string.Format(
                        Noark5Messages.DatesFirstAndLastRegistrationMessage_NumberOfRegistrations,
                        _registrationCount))
                }
            };

            if (_invalidRegistrationCreationDateCount > 0)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Error, 
                    new Location(ArkadeConstants.ArkivuttrekkXmlFileName, _invalidRegistrationCreationDateLocations),
                    string.Format(Noark5Messages.DatesFirstAndLastRegistrationMessage_NumberOfInvalidRegistrationCreationDates,
                        _invalidRegistrationCreationDateCount))
                );

            const string dateFormat = "dd.MM.yyyy";

            foreach (N5_27_ArchivePart archivePart in _archiveParts)
            {
                if (!archivePart.RegistrationCreationDates.Any())
                    continue;

                var testsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(""), string.Format(
                        Noark5Messages.DatesFirstAndLastRegistrationMessage_CreationDateFirstRegistration,
                        archivePart.RegistrationCreationDates.First().ToString(dateFormat))),
                    new(ResultType.Success, new Location(""), string.Format(
                        Noark5Messages.DatesFirstAndLastRegistrationMessage_CreationDateLastRegistration,
                        archivePart.RegistrationCreationDates.Last().ToString(dateFormat))),
                };

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testsResults
                    });
                else
                    testResultSet.TestsResults.AddRange(testsResults);
            }

            return testResultSet;
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
            {
                _invalidRegistrationCreationDateCount++;
                _invalidRegistrationCreationDateLocations.Add(eventArgs.LineNumber);
            }
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

        private class N5_27_ArchivePart : ArchivePart
        {
            public readonly SortedSet<DateTime> RegistrationCreationDates = new SortedSet<DateTime>();
        }
    }
}
