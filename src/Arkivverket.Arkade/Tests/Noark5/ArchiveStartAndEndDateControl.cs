using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #39
    /// </summary>
    public class ArchiveStartAndEndDateControl : Noark5XmlReaderBaseTest
    {
        private readonly SortedSet<DateTime> _registrationCreationDates;
        private readonly JournalHead _headPublicJournal;
        private readonly JournalHead _headRunningJournal;
        private readonly bool _periodSeparationIsSharp;

        public ArchiveStartAndEndDateControl(Archive archive)
        {
            _registrationCreationDates = new SortedSet<DateTime>();
            
            _headPublicJournal = Noark5TestHelper.GetJournalHead(ArkadeConstants.PublicJournalXmlFileName, archive);
            _headRunningJournal = Noark5TestHelper.GetJournalHead(ArkadeConstants.RunningJournalXmlFileName, archive);

            _periodSeparationIsSharp = Noark5TestHelper.PeriodSeparationIsSharp(archive);
        }

        public override string GetName()
        {
            return Noark5Messages.ArchiveStartAndEndDateControl;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            if (_headPublicJournal == null || _headRunningJournal == null)
            {
                testResults.Add(new TestResult(
                    ResultType.Error, new Location(string.Empty), Noark5Messages.CouldNotReadFromFiles)
                );

                return testResults;
            }
    
            var archiveDates = new StartAndEndDate(
                _registrationCreationDates.First(),
                _registrationCreationDates.Last()
            );
            var publicJournalDates = new StartAndEndDate(
                _headPublicJournal.JournalStartDate,
                _headPublicJournal.JournalEndDate
            );
            var runningJournalDates = new StartAndEndDate(
                _headRunningJournal.JournalStartDate,
                _headRunningJournal.JournalEndDate
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
            if (eventArgs.Path.Matches("opprettetDato", "registrering"))
            {
                string dateString = eventArgs.Value.Substring(0, 10); // Time part stripped off

                DateTime registrationCreatedTime;

                if (DateTime.TryParse(dateString, out registrationCreatedTime))
                    _registrationCreationDates.Add(registrationCreatedTime);
            }
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

            public StartAndEndDate(DateTime startDate, DateTime endDate)
            {
                _startDate = startDate;
                _endDate = endDate;
            }

            public string StartDate()
            {
                return _startDate.ToShortDateString();
            }

            public string EndDate()
            {
                return _endDate.ToShortDateString();
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
