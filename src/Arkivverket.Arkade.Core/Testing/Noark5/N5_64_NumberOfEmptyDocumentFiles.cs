using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_64_NumberOfEmptyDocumentFiles : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new(TestId.TestKind.Noark5, 64);

        private readonly ReadOnlyDictionary<string, DocumentFile> _documentFiles;

        private readonly ICollection<N5_64_ArchivePart> _archiveParts = new List<N5_64_ArchivePart>();
        private N5_64_ArchivePart _currentArchivePart;
        private N5_64_Folder _currentFolder;
        private N5_64_Registration _currentRegistration;
        private DocumentDescription _currentDocumentDescription;
        private DocumentObject _currentDocumentObject;

        public N5_64_NumberOfEmptyDocumentFiles(Archive archive)
        {
            _documentFiles = archive?.DocumentFiles;
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

            var totalNumberOfEmptyDocumentFiles = _archiveParts.Sum(a => a.NumberOfEmptyDocumentFiles);

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfEmptyDocumentFiles))
                }
            };

            if (totalNumberOfEmptyDocumentFiles == 0)
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
            foreach (N5_64_ArchivePart archivePart in _archiveParts)
            {
                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                    TestsResults = new List<TestResult>
                    {
                        new(ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.TotalResultNumber, archivePart.NumberOfEmptyDocumentFiles))
                    }
                };

                archivePartResultSet.TestsResults.AddRange(CreateTestResults(archivePart));

                yield return archivePartResultSet;
            }
        }

        private static IEnumerable<TestResult> CreateTestResults(N5_64_ArchivePart archivePart)
        {
            foreach (DocumentObject documentObject in archivePart.GetDocumentObjects())
            {
                yield return new TestResult(ResultType.Success,
                    new Location(ArkadeConstants.ArkivstrukturXmlFileName, documentObject.LineNumber),
                    documentObject.ToString());
            }
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new N5_64_ArchivePart();
                _currentFolder = null;
            }
            else if (eventArgs.NameEquals("mappe"))
            {
                var tmpFolder = new N5_64_Folder();
                if (eventArgs.Path.GetParent().Equals("mappe"))
                {
                    tmpFolder.ContainingFolder = _currentFolder;
                }
                _currentFolder = tmpFolder;
            }
            else if (eventArgs.NameEquals("registrering"))
            {
                _currentRegistration = new N5_64_Registration();
            }
            else if (eventArgs.NameEquals("dokumentbeskrivelse"))
            {
                _currentDocumentDescription = new DocumentDescription(_currentRegistration);
            }
            else if (eventArgs.NameEquals("dokumentobjekt"))
            {
                _currentDocumentObject = new DocumentObject(_currentDocumentDescription, eventArgs.LineNumber);
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
            else if (eventArgs.Path.Matches("referanseDokumentfil", "dokumentobjekt"))
            {
                _currentDocumentObject.FileName = eventArgs.Value;

                if (TryGetDocumentFile(eventArgs.Value, out DocumentFile documentFile))
                {
                    _currentDocumentObject.FileIsEmpty = documentFile.FileInfo.Length == 0;
                }
            }
        }

        private bool TryGetDocumentFile(string documentFileName, out DocumentFile documentFile)
        {
            return _documentFiles.TryGetValue(documentFileName.Replace('\\', '/'), out documentFile);
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
                _currentRegistration.DocumentDescriptions.Add(_currentDocumentDescription);
            }
            else if (eventArgs.NameEquals("dokumentobjekt"))
            {
                if (_currentDocumentObject.FileIsEmpty)
                {
                    _currentDocumentDescription.DocumentObjects.Add(_currentDocumentObject);
                }
            }
        }

        private class N5_64_ArchivePart : ArchivePart
        {
            private int? _numberOfEmptyDocumentFiles;

            public int NumberOfEmptyDocumentFiles =>
                _numberOfEmptyDocumentFiles ??= GetNumberOfMissingDocumentObjects();

            public ICollection<N5_64_Folder> Folders { get; } = new List<N5_64_Folder>();

            public IEnumerable<DocumentObject> GetDocumentObjects()
            {
                return Folders.SelectMany(f => f.GetDocumentDescriptions()).SelectMany(dd => dd.DocumentObjects);
            }

            private int GetNumberOfMissingDocumentObjects()
            {
                int amount = Folders.Sum(f => f.NumberOfEmptyDocumentFiles);
                _numberOfEmptyDocumentFiles = amount;
                return amount;
            }
        }

        private class N5_64_Folder : Folder
        {
            private int? _numberOfEmptyDocumentFiles;
            public int NumberOfEmptyDocumentFiles =>
                _numberOfEmptyDocumentFiles ??= GetNumberOfEmptyDocumentFiles();

            public IEnumerable<DocumentDescription> GetDocumentDescriptions()
            {
                return Folders.Cast<N5_64_Folder>().SelectMany(f => f.GetDocumentDescriptions()).Concat(
                    Registrations.Cast<N5_64_Registration>().SelectMany(r => r.DocumentDescriptions));
            }

            private int GetNumberOfEmptyDocumentFiles()
            {
                int amount = Registrations.Cast<N5_64_Registration>().Sum(r => r.NumberOfEmptyDocumentFiles) +
                             Folders.Cast<N5_64_Folder>().Sum(f => f.NumberOfEmptyDocumentFiles);
                _numberOfEmptyDocumentFiles = amount;
                return amount;
            }
        }

        private class N5_64_Registration : Registration
        {
            private int? _numberOfEmptyDocumentFiles;
            public int NumberOfEmptyDocumentFiles =>
                _numberOfEmptyDocumentFiles ??= GetNumberOfEmptyDocumentFiles();

            public ICollection<DocumentDescription> DocumentDescriptions { get; } = new List<DocumentDescription>();

            private int GetNumberOfEmptyDocumentFiles()
            {
                int amount = DocumentDescriptions.Sum(d => d.NumberOfEmptyDocumentFiles);
                _numberOfEmptyDocumentFiles = amount;
                return amount;
            }
        }

        private class DocumentDescription
        {
            public N5_64_Registration ContainingRegistration { get; }

            private int? _numberOfEmptyDocumentFiles;
            public int NumberOfEmptyDocumentFiles =>
                _numberOfEmptyDocumentFiles ??= GetNumberOfEmptyDocumentFiles();

            public string SystemId { get; set; }
            public string DocumentNumber { get; set; }

            public ICollection<DocumentObject> DocumentObjects { get; } = new List<DocumentObject>();

            public DocumentDescription(N5_64_Registration containingRegistration)
            {
                ContainingRegistration = containingRegistration;
            }

            private int GetNumberOfEmptyDocumentFiles()
            {
                int amount = DocumentObjects.Sum(d => d.FileIsEmpty ? 1 : 0);
                _numberOfEmptyDocumentFiles = amount;
                return amount;
            }
        }

        private class DocumentObject
        {
            private readonly DocumentDescription _containingDocumentDescription;

            public long LineNumber { get; }
            public string FileName { get; set; }
            public bool FileIsEmpty { get; set; }

            public DocumentObject(DocumentDescription containingDocumentDescription, long lineNumber)
            {
                _containingDocumentDescription = containingDocumentDescription;
                LineNumber = lineNumber;
            }

            public override string ToString()
            {
                return string.Format(
                    Noark5Messages.DocumentFileIsEmptyMessage,
                    FileName,
                    _containingDocumentDescription.SystemId,
                    _containingDocumentDescription.ContainingRegistration.RegistrationId,
                    _containingDocumentDescription.DocumentNumber
                );
            }
        }
    }
}
