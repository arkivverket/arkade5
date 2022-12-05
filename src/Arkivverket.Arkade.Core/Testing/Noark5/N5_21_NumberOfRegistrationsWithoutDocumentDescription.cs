using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_21_NumberOfRegistrationsWithoutDocumentDescription : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 21);

        private readonly Dictionary<ArchivePart, List<N5_21_Registration>> _registrationsWithoutDocumentDescriptionPerArchivePart = new();
        private ArchivePart _currentArchivePart = new();
        private int _totalNumberOfMissingDocumentDescriptions;
        private Folder _currentFolder;
        private N5_21_Registration _currentRegistration;

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
            bool multipleArchiveParts = _registrationsWithoutDocumentDescriptionPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(Noark5Messages.TotalResultNumber,
                        _totalNumberOfMissingDocumentDescriptions))
                }
            };

            if (_totalNumberOfMissingDocumentDescriptions == 0)
                return testResultSet;

            if (multipleArchiveParts)
            {
                testResultSet.TestResultSets.AddRange(CreateTestResultSets());
            }
            else
            {
                testResultSet.TestsResults.AddRange(
                    CreateTestResults(_registrationsWithoutDocumentDescriptionPerArchivePart.First().Value));
            }

            return testResultSet;
        }

        private IEnumerable<TestResultSet> CreateTestResultSets()
        {
            foreach ((ArchivePart archivePart, List<N5_21_Registration> registrations) in
                     _registrationsWithoutDocumentDescriptionPerArchivePart)
            {
                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                    TestsResults = new List<TestResult>
                    {
                        new(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.TotalResultNumber, registrations.Count))
                    }
                };

                archivePartResultSet.TestsResults.AddRange(CreateTestResults(registrations));

                yield return archivePartResultSet;
            }
        }

        private static IEnumerable<TestResult> CreateTestResults(IEnumerable<N5_21_Registration> registrations)
        {
            return registrations.Select(registration => new TestResult(ResultType.Success,
                new Location(ArkadeConstants.ArkivstrukturXmlFileName, registration.LineNumber),
                registration.ToString()));
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentbeskrivelse"))
                _currentRegistration.HasDocumentDescription = true;

            else if (eventArgs.NameEquals("mappe"))
            {
                _currentFolder = new Folder(_currentFolder);
            }

            else if (eventArgs.NameEquals("registrering"))
                _currentRegistration = new N5_21_Registration(_currentFolder, eventArgs.LineNumber);
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                _registrationsWithoutDocumentDescriptionPerArchivePart.Add(_currentArchivePart, new List<N5_21_Registration>());
            }
            else if (eventArgs.Path.Matches("tittel", "arkivdel"))
            {
                _currentArchivePart.Name = eventArgs.Value;
            }
            else if (Noark5TestHelper.IdentifiesFolderStatus(eventArgs))
            {
                _currentFolder.Status = eventArgs.Value;
            }
            else if (eventArgs.Path.Matches("systemID", "registrering"))
            {
                _currentRegistration.SystemId = eventArgs.Value;
            }
            else if (eventArgs.Path.Matches("registreringsID", "registrering"))
            {
                _currentRegistration.RegistrationId = eventArgs.Value;
            }
            else if (Noark5TestHelper.IdentifiesRegistrationStatus(eventArgs))
            {
                _currentRegistration.Status = eventArgs.Value;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering"))
            {
                if (_currentRegistration.ShallBeReported)
                {
                    _totalNumberOfMissingDocumentDescriptions++;
                    _registrationsWithoutDocumentDescriptionPerArchivePart[_currentArchivePart].Add(_currentRegistration);
                }
            }

            if(eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new ArchivePart();
            }
        }

        private class N5_21_Registration : Registration
        {
            public long LineNumber { get; }
            public string SystemId { get; set; }
            public bool HasDocumentDescription { get; set; }
            public bool ShallBeReported => !Utgaar() && !HasDocumentDescription;

            public N5_21_Registration(Folder containingFolder, long lineNumber) : base(containingFolder)
            {
                LineNumber = lineNumber;
            }

            public override string ToString()
            {
                return string.Format(Noark5Messages.RegistrationSystemIdAndRegistrationId, SystemId, RegistrationId);
            }
        }
    }
}
