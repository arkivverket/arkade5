using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private readonly ICollection<N5_21_ArchivePart> _archiveParts = new List<N5_21_ArchivePart>();
        private N5_21_ArchivePart _currentArchivePart = new();
        private N5_21_Folder _currentFolder;
        private N5_21_Registration _currentRegistration;
        private int _totalNumberOfMissingDocumentDescriptions;

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

            _totalNumberOfMissingDocumentDescriptions = _archiveParts.Sum(a => a.NumberOfRegistrationsWithoutDocumentDescriptions);

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
                    CreateTestResults(_currentArchivePart.GetRegistrations()));
            }

            return testResultSet;
        }

        private IEnumerable<TestResultSet> CreateTestResultSets()
        {
            foreach (N5_21_ArchivePart archivePart in _archiveParts)
            {
                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                    TestsResults = new List<TestResult>
                    {
                        new(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.TotalResultNumber, archivePart.NumberOfRegistrationsWithoutDocumentDescriptions))
                    }
                };

                archivePartResultSet.TestsResults.AddRange(CreateTestResults(archivePart.GetRegistrations()));

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
            if (eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new N5_21_ArchivePart();
            }
            else if (eventArgs.NameEquals("mappe"))
            {
                var tempFolder = new N5_21_Folder();
                if (eventArgs.Path.GetParent().Equals("mappe"))
                {
                    tempFolder.ContainingFolder = _currentFolder;
                }
                _currentFolder = tempFolder;
            }
            else if (eventArgs.NameEquals("registrering"))
            {
                _currentRegistration = new N5_21_Registration(eventArgs.LineNumber);
            }
            else if (eventArgs.NameEquals("dokumentbeskrivelse"))
            {
                _currentRegistration.HasDocumentDescription = true;
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
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
            if (eventArgs.NameEquals("arkivdel"))
            {
                _archiveParts.Add(_currentArchivePart);
                _currentFolder = null;
            }
            else if (eventArgs.NameEquals("mappe"))
            {
                if (_currentFolder.Utgaar)
                    return;

                if (_currentFolder.ContainingFolder != null)
                {
                    _currentFolder.ContainingFolder.Folders.Add(_currentFolder);
                }
                else
                {
                    _currentArchivePart.Folders.Add(_currentFolder);
                }
            }
            else if (eventArgs.NameEquals("registrering"))
            {
                if (_currentRegistration.ShallBeReported)
                    _currentFolder.Registrations.Add(_currentRegistration);
            }
        }

        private class N5_21_ArchivePart : ArchivePart
        {
            private int? _numberOfRegistrationsWithoutDocumentDescriptions;
            public int NumberOfRegistrationsWithoutDocumentDescriptions =>
                _numberOfRegistrationsWithoutDocumentDescriptions ??=
                    Folders.Sum(f => f.NumberOfRegistrationsWithoutDocumentDescriptions);

            public ICollection<N5_21_Folder> Folders { get; } = new Collection<N5_21_Folder>();

            public IEnumerable<N5_21_Registration> GetRegistrations()
            {
                return Folders.SelectMany(f => f.GetRegistrations());
            }
        }

        private class N5_21_Folder : Folder
        {
            private int? _numberOfRegistrationsWithoutDocumentDescriptions;
            public int NumberOfRegistrationsWithoutDocumentDescriptions =>
                _numberOfRegistrationsWithoutDocumentDescriptions ??=
                    Folders.Cast<N5_21_Folder>().Sum(f => f.NumberOfRegistrationsWithoutDocumentDescriptions) +
                    Registrations.Cast<N5_21_Registration>().Sum(r => r.ShallBeReported ? 1 : 0);

            public IEnumerable<N5_21_Registration> GetRegistrations()
            {
                return Folders.SelectMany(f => ((N5_21_Folder)f).GetRegistrations())
                    .Concat(Registrations.Cast<N5_21_Registration>());
            }
        }

        private class N5_21_Registration : Registration
        {
            public long LineNumber { get; }
            public string SystemId { get; set; }
            public bool HasDocumentDescription { get; set; }
            public bool ShallBeReported => !Utgaar && !HasDocumentDescription;


            public N5_21_Registration(long lineNumber)
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
