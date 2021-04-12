using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_60_ArchiveStartAndEndDateControl : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 60);

        private readonly Archive _archive;
        private readonly SortedSet<DateTime> _registrationCreationDates;
        private readonly bool _periodSeparationIsSharp;

        public N5_60_ArchiveStartAndEndDateControl(Archive archive)
        {
            _archive = archive;
            _registrationCreationDates = new SortedSet<DateTime>();
            _periodSeparationIsSharp = Noark5TestHelper.PeriodSeparationIsSharp(archive);
        }

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

            JournalHead headPublicJournal;
            JournalHead headRunningJournal;

            ArchiveXmlFile publicJournal = _archive.GetArchiveXmlFile(ArkadeConstants.PublicJournalXmlFileName);
            ArchiveXmlFile runningJournal = _archive.GetArchiveXmlFile(ArkadeConstants.RunningJournalXmlFileName);

            try
            {
                headPublicJournal = JournalGuillotine.Behead(publicJournal);
                headRunningJournal = JournalGuillotine.Behead(runningJournal);
            }
            catch
            {
                testResults.Add(new TestResult(
                    ResultType.Error, new Location(string.Empty), Noark5Messages.CouldNotReadFromFiles)
                );

                return testResults;
            }

            if (!_registrationCreationDates.Any())
            {
                testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                    Noark5Messages.ArchiveStartAndEndDateControlMessage_NoArchiveDatesFound));

                return testResults;
            }

            var archiveDates = new StartAndEndDate(
                _registrationCreationDates.First(),
                _registrationCreationDates.Last()
            );
            var publicJournalDates = new StartAndEndDate(
                headPublicJournal.JournalStartDate,
                headPublicJournal.JournalEndDate
            );
            var runningJournalDates = new StartAndEndDate(
                headRunningJournal.JournalStartDate,
                headRunningJournal.JournalEndDate
            );

            testResults.AddRange(new[]
            {
                new TestResult(ResultType.Success, new Location(""), string.Format(
                    Noark5Messages.ArchiveStartAndEndDateControlMessage_DatesArchive,
                    archiveDates.StartDate(), archiveDates.EndDate()
                )),
                new TestResult(ResultType.Success, new Location(""), string.Format(
                    Noark5Messages.ArchiveStartAndEndDateControlMessage_DatesPublicJournal,
                    publicJournalDates.StartDate(), publicJournalDates.EndDate()
                )),
                new TestResult(ResultType.Success, new Location(""), string.Format(
                    Noark5Messages.ArchiveStartAndEndDateControlMessage_DatesRunningJournal,
                    runningJournalDates.StartDate(), runningJournalDates.EndDate()
                ))
            });

            if (!StartAndEndDate.Equals(publicJournalDates, runningJournalDates))
                testResults.Add(new TestResult(ResultType.Error, new Location(""),
                    Noark5Messages.ArchiveStartAndEndDateControlMessage_UnEqualJournalDates));

            
            if (_periodSeparationIsSharp && !StartAndEndDate.Equals(archiveDates, publicJournalDates, runningJournalDates))
                testResults.Add(new TestResult(ResultType.Error, new Location(""),
                    Noark5Messages.ArchiveStartAndEndDateControlMessage_UnEqualJournalAndArchiveDates));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("journaldato", "registrering") &&
                Noark5TestHelper.TryParseValidXmlDate(eventArgs.Value, out DateTime registrationCreatedTime))
                    _registrationCreationDates.Add(registrationCreatedTime);
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class StartAndEndDate
        {
            private DateTime _startDate;
            private DateTime _endDate;
            private readonly string _dateFormat;

            public StartAndEndDate(DateTime startDate, DateTime endDate)
            {
                _startDate = startDate.Date;
                _endDate = endDate.Date;
                _dateFormat = "dd.MM.yyyy";
            }

            public string StartDate()
            {
                return _startDate.ToString(_dateFormat);
            }

            public string EndDate()
            {
                return _endDate.ToString(_dateFormat);
            }

            public bool Equals(StartAndEndDate startAndEndDate)
            {
                return startAndEndDate._startDate.Equals(_startDate)
                       && startAndEndDate._endDate.Equals(_endDate);
            }

            public static bool Equals(params StartAndEndDate[] startAndEndDates)
            {
                return startAndEndDates.All(s => s.Equals(startAndEndDates.First()));
            }
        }
    }
}
