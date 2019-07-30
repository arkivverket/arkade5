using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_16_NumberOfRegistrations : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 16);
        private readonly List<ArchivePart> _archiveParts = new List<ArchivePart>();
        private ArchivePart _currentArchivePart = new ArchivePart();

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

            var totalNumberOfRegistrations = 0;

            bool hasMultipleArchiveParts = _archiveParts.Count > 1;

            foreach (ArchivePart archivePart in _archiveParts)
            {
                string archivePartMessagePrefix = hasMultipleArchiveParts
                    ? string.Format(Noark5Messages.ArchivePartSystemId, archivePart.SystemId) + " - "
                    : string.Empty;

                testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    archivePartMessagePrefix + string.Format(
                        Noark5Messages.TotalResultNumber, archivePart.TotalRegistrationsCount
                    )));

                foreach(var registration in archivePart.Registrations)
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                        archivePartMessagePrefix + string.Format(
                            Noark5Messages.NumberOfTypeRegisters, Noark5TestHelper.StripNamespace(registration.Key), registration.Value
                        )));
                }

                totalNumberOfRegistrations += archivePart.TotalRegistrationsCount;
            }

            if (hasMultipleArchiveParts)
            {
                testResults.Insert(0, new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.TotalResultNumber, totalNumberOfRegistrations)));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering"))
                _currentArchivePart.TotalRegistrationsCount++;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesRegistration(eventArgs))
            {
                var registrationType = eventArgs.Value;

                if (_currentArchivePart.Registrations.ContainsKey(registrationType))
                    _currentArchivePart.Registrations[registrationType]++;
                else
                    _currentArchivePart.Registrations.Add(registrationType, 1);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart = new ArchivePart {SystemId = eventArgs.Value};
                _archiveParts.Add(_currentArchivePart);
            }
        }

        private class ArchivePart
        {
            public string SystemId { get; set; }
            public int TotalRegistrationsCount;
            public Dictionary<string, int> Registrations = new Dictionary<string, int>();
        }
    }
}
