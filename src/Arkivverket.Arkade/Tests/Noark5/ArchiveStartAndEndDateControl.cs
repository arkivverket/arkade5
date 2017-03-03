using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.ExternalModels.PublicJournal;
using Arkivverket.Arkade.ExternalModels.RunningJournal;
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
        private readonly offentligJournal _publicJournal;
        private readonly loependeJournal _runningJournal;
        private readonly addml _archiveExtraction;
        private static List<string> _filesNotFound;

        public ArchiveStartAndEndDateControl(Archive archive)
        {
            _registrationCreationDates = new SortedSet<DateTime>();

            _filesNotFound = new List<string>();
            _publicJournal = Deserialize<offentligJournal>(ArkadeConstants.PublicJournalXmlFileName, archive);
            _runningJournal = Deserialize<loependeJournal>(ArkadeConstants.RunningJournalXmlFileName, archive);
            _archiveExtraction = Deserialize<addml>(ArkadeConstants.ArkivuttrekkXmlFileName, archive);
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

            if (_filesNotFound.Any())
            {
                foreach (var fileNotFound in _filesNotFound)
                    testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        string.Format(Noark5Messages.FileNotFound, fileNotFound)));

                return testResults;
            }

            var archiveDates = new StartAndEndDate(
                _registrationCreationDates.First(),
                _registrationCreationDates.Last()
            );
            var publicJournalDates = new StartAndEndDate(
                _publicJournal.journalhode.journalStartDato,
                _publicJournal.journalhode.journalSluttDato
            );
            var runningJournalDates = new StartAndEndDate(
                _runningJournal.journalhode.journalStartDato,
                _runningJournal.journalhode.journalSluttDato
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

            if (PeriodSeparationIsSharp() &&
                !StartAndEndDate.Equals(archiveDates, publicJournalDates, runningJournalDates))
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

        private bool PeriodSeparationIsSharp()
        {
            dataObject archiveExtractionElement = _archiveExtraction.dataset[0].dataObjects.dataObject[0];
            property infoElement = archiveExtractionElement.properties[0];
            property additionalInfoElement = infoElement.properties[1];
            property periodProperty =
                additionalInfoElement.properties.FirstOrDefault(p => p.name == "periode");

            property inboundSeparation = periodProperty.properties[0];
            property outboundSeparation = periodProperty.properties[1];

            return inboundSeparation.value.Equals("skarp") && outboundSeparation.value.Equals("skarp");
        }

        private static T Deserialize<T>(string xmlFile, Archive archive)
        {
            try
            {
                return SerializeUtil.DeserializeFromFile<T>(
                    archive.WorkingDirectory.Content().WithFile(xmlFile).FullName
                );
            }
            catch (Exception)
            {
                _filesNotFound.Add(xmlFile);
                return default(T);
            }
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
