using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 24);

        private static int _totalNumberOfMissingDocumentObjects;
        private readonly ICollection<N5_24_ArchivePart> _archiveParts = new List<N5_24_ArchivePart>();
        private N5_24_ArchivePart _currentArchivePart;
        private N5_24_Folder _currentFolder;
        private N5_24_Registration _currentRegistration;
        private DocumentDescription _currentDocumentDescription;
        private bool _documentDescriptionWithoutDocumentObjectAndPhysicalDocumentMediumIsFound = false;

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

            _totalNumberOfMissingDocumentObjects = _archiveParts.Sum(a => a.NumberOfMissingDocumentObjects);

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _totalNumberOfMissingDocumentObjects))
                }
            };

            if (_totalNumberOfMissingDocumentObjects == 0 && 
                !_documentDescriptionWithoutDocumentObjectAndPhysicalDocumentMediumIsFound)
            {
                return testResultSet;
            }

            if (multipleArchiveParts)
            {
                testResultSet.TestResultSets.AddRange(CreateTestResultSets());
            }
            else
            {
                testResultSet.TestsResults.AddRange(CreateTestResults(_currentArchivePart));
            }

            return testResultSet;
        }

        private IEnumerable<TestResultSet> CreateTestResultSets()
        {
            foreach (N5_24_ArchivePart archivePart in _archiveParts)
            {
                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                    TestsResults = new List<TestResult>
                    {
                        new(ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.TotalResultNumber, archivePart.NumberOfMissingDocumentObjects))
                    }
                };

                archivePartResultSet.TestsResults.AddRange(CreateTestResults(archivePart));

                if (archivePart.NumberOfDocumentDescriptionsWithPhysicalStorageMedium > 0)
                {
                    archivePartResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(ArkadeConstants.ArkivstrukturXmlFileName),
                        string.Format(Noark5Messages.DocumentDescriptionsWithoutDocumentObjectsAndPhysicalStorage,
                            archivePart.NumberOfDocumentDescriptionsWithPhysicalStorageMedium)));
                }

                yield return archivePartResultSet;
            }
        }

        private IEnumerable<TestResult> CreateTestResults(N5_24_ArchivePart archivePart)
        {
            foreach (DocumentDescription documentDescription in archivePart.GetDocumentDescriptions())
            {
                yield return new TestResult(ResultType.Success,
                    new Location(ArkadeConstants.ArkivstrukturXmlFileName, documentDescription.LineNumber),
                    documentDescription.ToString());
            }

            if (archivePart.NumberOfDocumentDescriptionsWithPhysicalStorageMedium > 0)
            {
                yield return new TestResult(ResultType.Success, new Location(ArkadeConstants.ArkivstrukturXmlFileName),
                    string.Format(Noark5Messages.DocumentDescriptionsWithoutDocumentObjectsAndPhysicalStorage,
                        archivePart.NumberOfDocumentDescriptionsWithPhysicalStorageMedium));
            }
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new N5_24_ArchivePart();
                _currentFolder = null;
            }
            else if (eventArgs.NameEquals("mappe"))
            {
                var tmpFolder = new N5_24_Folder();
                if (eventArgs.Path.GetParent().Equals("mappe"))
                {
                    tmpFolder.ContainingFolder = _currentFolder;
                }
                _currentFolder = tmpFolder;
            }
            else if (eventArgs.NameEquals("registrering"))
            {
                _currentRegistration = new N5_24_Registration();
            }
            else if(eventArgs.NameEquals("dokumentbeskrivelse"))
            {
                _currentDocumentDescription = new DocumentDescription(_currentRegistration, eventArgs.LineNumber);
            }
            else if (eventArgs.NameEquals("dokumentobjekt") && eventArgs.Path.GetParent() == "dokumentbeskrivelse")
            {
                _currentDocumentDescription.HasDocumentObject = true;
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
            else if (Noark5TestHelper.IdentifiesRegistrationStatus(eventArgs))
            {
                _currentRegistration.Status = eventArgs.Value;
            }
            else if (eventArgs.Path.Matches("registreringsID", "registrering"))
            {
                _currentRegistration.RegistrationId = eventArgs.Value;
            }
            else if (eventArgs.Path.Matches("systemID", "dokumentbeskrivelse"))
            {
                _currentDocumentDescription.SystemId = eventArgs.Value;
            }
            else if (eventArgs.Path.Matches("dokumentnummer", "dokumentbeskrivelse"))
            {
                _currentDocumentDescription.DocumentNumber = eventArgs.Value;
            }
            else if (eventArgs.Path.Matches("dokumentmedium", "dokumentbeskrivelse"))
            {
                _currentDocumentDescription.DokumentMedium = eventArgs.Value;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
            {
                _archiveParts.Add(_currentArchivePart);
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
                if (_currentRegistration.Utgaar)
                    return;

                _currentFolder.Registrations.Add(_currentRegistration);
            }
            else if (eventArgs.NameEquals("dokumentbeskrivelse"))
            {
                if (_currentDocumentDescription.ShallBeReported)
                {
                    _currentRegistration.DocumentDescriptions.Add(_currentDocumentDescription);
                }
                else if (_currentDocumentDescription.DokumentMedium?.ToLower() is "fysisk medium" or "fysisk arkiv")
                {
                    _currentArchivePart.NumberOfDocumentDescriptionsWithPhysicalStorageMedium++;
                    _documentDescriptionWithoutDocumentObjectAndPhysicalDocumentMediumIsFound = true;
                }
            }
        }

        private class N5_24_ArchivePart : ArchivePart
        {
            private int? _numberOfMissingDocumentObjects;

            public int NumberOfMissingDocumentObjects =>
                _numberOfMissingDocumentObjects ??= GetNumberOfMissingDocumentObjects();

            public long NumberOfDocumentDescriptionsWithPhysicalStorageMedium { get; set; }
            public ICollection<N5_24_Folder> Folders { get; } = new List<N5_24_Folder>();

            public IEnumerable<DocumentDescription> GetDocumentDescriptions()
            {
                return Folders.SelectMany(f => f.GetDocumentDescriptions());
            }

            private int GetNumberOfMissingDocumentObjects()
            {
                int amount = Folders.Sum(f => f.NumberOfMissingDocumentObjects);
                _numberOfMissingDocumentObjects = amount;
                return amount;
            }
        }

        private class N5_24_Folder : Folder
        {
            private int? _numberOfMissingDocumentObjects;
            public int NumberOfMissingDocumentObjects =>
                _numberOfMissingDocumentObjects ??= GetNumberOfMissingDocumentObjects();

            public IEnumerable<DocumentDescription> GetDocumentDescriptions()
            {
                return Folders.Cast<N5_24_Folder>().SelectMany(f => f.GetDocumentDescriptions()).Concat(
                    Registrations.Cast<N5_24_Registration>().SelectMany(r => r.DocumentDescriptions));
            }

            private int GetNumberOfMissingDocumentObjects()
            {
                int amount = Registrations.Cast<N5_24_Registration>().Sum(r => r.NumberOfMissingDocumentObjects) +
                             Folders.Cast<N5_24_Folder>().Sum(f => f.NumberOfMissingDocumentObjects);
                _numberOfMissingDocumentObjects = amount;
                return amount;
            }
        }

        private class N5_24_Registration : Registration
        {
            private int? _numberOfMissingDocumentObjects;
            public int NumberOfMissingDocumentObjects => 
                _numberOfMissingDocumentObjects ??= GetNumberOfMissingDocumentObjects();

            public ICollection<DocumentDescription> DocumentDescriptions { get; } = new List<DocumentDescription>();

            private int GetNumberOfMissingDocumentObjects()
            {
                int amount = DocumentDescriptions.Sum(d => d.ShallBeReported ? 1 : 0);
                _numberOfMissingDocumentObjects = amount;
                return amount;
            }
        }

        private class DocumentDescription
        {
            private readonly N5_24_Registration _containingRegistration;

            public long LineNumber { get; }
            public string SystemId { get; set; }
            public string DocumentNumber { get; set; }
            public string DokumentMedium { get; set; }
            public bool HasDocumentObject { get; set; }

            public bool ShallBeReported => !HasDocumentObject &&
                                           DokumentMedium?.ToLower() is not ("fysisk medium" or "fysisk arkiv");

            public DocumentDescription(N5_24_Registration containingRegistration, long lineNumber)
            {
                _containingRegistration = containingRegistration;
                LineNumber = lineNumber;
            }

            public override string ToString()
            {
                return string.Format(Noark5Messages.DocumentDescriptionSystemIdRegistrationIdAndDocumentNumber,
                    SystemId, _containingRegistration.RegistrationId, DocumentNumber);
            }
        }
    }
}