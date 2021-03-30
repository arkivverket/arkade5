using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_16_NumberOfRegistrations : Noark5XmlReaderBaseTest
    {
        private readonly Archive _archive;
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 16);
        private readonly List<N5_16_ArchivePart> _archiveParts = new List<N5_16_ArchivePart>();
        private N5_16_ArchivePart _currentArchivePart = new N5_16_ArchivePart();
        private Stack<string> _registrationTypes = new Stack<string>();

        public N5_16_NumberOfRegistrations(Archive archive)
        {
            _archive = archive;
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

            var totalNumberOfRegistrations = 0;

            bool hasMultipleArchiveParts = _archiveParts.Count > 1;

            foreach (N5_16_ArchivePart archivePart in _archiveParts)
            {
                string archivePartMessagePrefix = hasMultipleArchiveParts
                    ? string.Format(Noark5Messages.ArchivePartSystemId, archivePart.SystemId, archivePart.Name) + " - "
                    : string.Empty;

                testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    archivePartMessagePrefix + string.Format(
                        Noark5Messages.TotalResultNumber, archivePart.TotalRegistrationsCount
                    )));

                foreach (var registration in archivePart.Registrations)
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                        archivePartMessagePrefix + string.Format(
                            Noark5Messages.NumberOfTypeRegistrations, Noark5TestHelper.StripNamespace(registration.Key), registration.Value
                        )));
                }

                totalNumberOfRegistrations += archivePart.TotalRegistrationsCount;
            }

            int documentedNumberOfRegistrations = GetDocumentedNumberOfRegistrations();

            if (totalNumberOfRegistrations != documentedNumberOfRegistrations)
                testResults.Add(new TestResult(ResultType.Error, new Location(ArkadeConstants.ArkivuttrekkXmlFileName), string.Format(
                    Noark5Messages.NumberOfRegistrations_DocumentedAndActualMismatch, documentedNumberOfRegistrations, totalNumberOfRegistrations)));

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
            {
                _currentArchivePart.TotalRegistrationsCount++;
                _registrationTypes.Push("registrering");
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesRegistration(eventArgs))
            {
                _registrationTypes.Pop();
                _registrationTypes.Push(eventArgs.Value);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering"))
            {
                var registrationType = _registrationTypes.Pop();

                if (_currentArchivePart.Registrations.ContainsKey(registrationType))
                    _currentArchivePart.Registrations[registrationType]++;
                else
                    _currentArchivePart.Registrations.Add(registrationType, 1);
            }

            if (eventArgs.NameEquals("arkivdel"))
            {
                _archiveParts.Add(_currentArchivePart);
                _currentArchivePart = new N5_16_ArchivePart();
        }
        }


        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart = new N5_16_ArchivePart { SystemId = eventArgs.Value };

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        private int GetDocumentedNumberOfRegistrations()
        {
            var addml = SerializeUtil.DeserializeFromFile<addml>(_archive.AddmlXmlUnit.File);

            string numberOfRegistrations = addml.dataset[0].dataObjects.dataObject[0]
                .dataObjects.dataObject[0].properties.FirstOrDefault(
                    p => p.name == "info").properties.Where(
                    p => p.name == "numberOfOccurrences").FirstOrDefault(
                    p => p.value.Equals("registrering")).properties.FirstOrDefault(
                    p => p.name.Equals("value"))
                .value;

            return int.Parse(numberOfRegistrations);
        }

        private class N5_16_ArchivePart : ArchivePart
        {
            public int TotalRegistrationsCount;
            public Dictionary<string, int> Registrations = new Dictionary<string, int>();
        }
    }
}
