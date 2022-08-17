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
        private readonly List<N5_16_ArchivePart> _archiveParts = new();
        private N5_16_ArchivePart _currentArchivePart = new();
        private Stack<string> _registrationTypes = new();

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

        protected override TestResultSet GetTestResults()
        {
            bool multipleArchiveParts = _archiveParts.Count > 1;

            int totalNumberOfRegistrations = _archiveParts.Sum(a => a.TotalRegistrationsCount);

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfRegistrations))
                }
            };

            int documentedNumberOfRegistrations = GetDocumentedNumberOfRegistrations();

            if (totalNumberOfRegistrations != documentedNumberOfRegistrations)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                    new Location(ArkadeConstants.ArkivstrukturXmlFileName),
                    string.Format(Noark5Messages.NumberOfRegistrations_DocumentedAndActualMismatch,
                        documentedNumberOfRegistrations, totalNumberOfRegistrations)));

            if (totalNumberOfRegistrations == 0)
                return testResultSet;

            foreach (N5_16_ArchivePart archivePart in _archiveParts)
            {
                var testResults = new List<TestResult>();

                if (multipleArchiveParts)
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOf, archivePart.TotalRegistrationsCount)));

                foreach ((string registrationType, int numberOfRegistrations) in archivePart.Registrations)
                    testResults.Add(new TestResult(
                        ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.NumberOfTypeRegistrations,
                            Noark5TestHelper.StripNamespace(registrationType), numberOfRegistrations)));

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                else
                    testResultSet.TestsResults.AddRange(testResults);
            }

            return testResultSet;
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
                _currentArchivePart = new N5_16_ArchivePart {SystemId = eventArgs.Value};

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        private int GetDocumentedNumberOfRegistrations()
        {
            addml addml = _archive.AddmlInfo.Addml;

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
