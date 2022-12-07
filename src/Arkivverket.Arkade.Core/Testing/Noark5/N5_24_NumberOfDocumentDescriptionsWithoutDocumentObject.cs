using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 24);

        private int _totalNumberOfMissingDocumentObjects;
        private N5_24_ArchivePart _currentArchivePart;
        private Folder _currentFolder;
        private Registration _currentRegistration;
        private DocumentDescription _currentDocumentDescription;
        private readonly Dictionary<N5_24_ArchivePart, List<DocumentDescription>> _documentDescriptionsWithoutDocumentObjectPerArchivePart = new();
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
            bool multipleArchiveParts = _documentDescriptionsWithoutDocumentObjectPerArchivePart.Count > 1;

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
            foreach ((N5_24_ArchivePart archivePart, List<DocumentDescription> documentDescriptions) in
                     _documentDescriptionsWithoutDocumentObjectPerArchivePart)
            {
                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                    TestsResults = new List<TestResult>
                    {
                        new(ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.TotalResultNumber, documentDescriptions.Count))
                    }
                };

                archivePartResultSet.TestsResults.AddRange(CreateTestResults(archivePart));

                yield return archivePartResultSet;
            }
        }

        private IEnumerable<TestResult> CreateTestResults(N5_24_ArchivePart archivePart)
        {
            foreach (DocumentDescription documentDescription in 
                     _documentDescriptionsWithoutDocumentObjectPerArchivePart[archivePart])
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
            }
            else if (eventArgs.NameEquals("mappe"))
            {
                _currentFolder = new Folder(_currentFolder);
            }
            else if (eventArgs.NameEquals("registrering"))
            {
                _currentRegistration = new Registration(_currentFolder);
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
                _documentDescriptionsWithoutDocumentObjectPerArchivePart.Add(_currentArchivePart, new List<DocumentDescription>());
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
            if (eventArgs.NameEquals("dokumentbeskrivelse"))
            {
                if (_currentDocumentDescription.ShallBeReported)
                {
                    _totalNumberOfMissingDocumentObjects++;
                    _documentDescriptionsWithoutDocumentObjectPerArchivePart[_currentArchivePart].Add(_currentDocumentDescription);
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
            public long NumberOfDocumentDescriptionsWithPhysicalStorageMedium { get; set; }
        }

        private class DocumentDescription
        {
            private readonly Registration _containingRegistration;

            public long LineNumber { get; }
            public string SystemId { get; set; }
            public string DocumentNumber { get; set; }
            public string DokumentMedium { get; set; }
            public bool HasDocumentObject { get; set; }

            public bool ShallBeReported => !_containingRegistration.Utgaar() &&
                                           !HasDocumentObject &&
                                           DokumentMedium?.ToLower() is not ("fysisk medium" or "fysisk arkiv");

            public DocumentDescription(Registration containingRegistration, long lineNumber)
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