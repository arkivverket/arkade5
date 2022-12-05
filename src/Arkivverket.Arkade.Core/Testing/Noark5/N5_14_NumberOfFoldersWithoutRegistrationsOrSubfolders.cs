using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 14);

        private int _noRegistrationOrSubfolderCount;
        private ArchivePart _currentArchivePart = new();
        private Folder _currentFolder;
        private readonly Stack<Folder> _folderStack = new();
        private readonly Dictionary<ArchivePart, List<Folder>> _foldersPerArchivePart = new();

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
            var multipleArchiveParts = _foldersPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _noRegistrationOrSubfolderCount))
                }
            };

            if (_noRegistrationOrSubfolderCount == 0)
                return testResultSet;

            if (multipleArchiveParts)
                testResultSet.TestResultSets.AddRange(CreateTestResultSets());
            else
                testResultSet.TestsResults.AddRange(CreateTestResults(_foldersPerArchivePart.First().Value));
            
            return testResultSet;
        }

        private IEnumerable<TestResultSet> CreateTestResultSets()
        {
            foreach ((ArchivePart archivePart, List<Folder> folders) in _foldersPerArchivePart)
            {
                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                    TestsResults = new List<TestResult>
                    {
                        new(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.TotalResultNumber, folders.Count))
                    }
                };

                archivePartResultSet.TestsResults.AddRange(CreateTestResults(folders));

                yield return archivePartResultSet;
            }
        }

        private static IEnumerable<TestResult> CreateTestResults(IEnumerable<Folder> folders)
        {
            return folders.Select(folder => new TestResult(ResultType.Success,
                new Location(ArkadeConstants.ArkivstrukturXmlFileName, folder.LineNumber),
                string.Format(folder.ToString())));
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering"))
                _currentFolder.HasSubFolderOrRegistration = true;

            if (eventArgs.NameEquals("mappe"))
            {
                if (eventArgs.Path.GetParent().Equals("mappe"))
                    _currentFolder.HasSubFolderOrRegistration = true;
                
                _currentFolder = new Folder(eventArgs.LineNumber);
                _folderStack.Push(_currentFolder);
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
                _foldersPerArchivePart.Add(_currentArchivePart, new List<Folder>());
            }
            else if (eventArgs.Path.Matches("tittel", "arkivdel"))
            {
                _currentArchivePart.Name = eventArgs.Value;
            }
            else if (eventArgs.Path.Matches("systemID", "mappe"))
            {
                _currentFolder.SystemId = eventArgs.Value;
            }
            else if (eventArgs.Path.Matches("mappeID", "mappe"))
            {
                _currentFolder.MappeId = eventArgs.Value;
            }
            else if (eventArgs.Path.Matches("saksstatus", "mappe"))
            {
                _currentFolder.Status = eventArgs.Value;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();

            if (!eventArgs.NameEquals("mappe"))
                return;

            Folder folder = _folderStack.Pop();
            if (folder.HasSubFolderOrRegistration || folder.Status?.ToLower() == "utgår")
                return;
            
            _noRegistrationOrSubfolderCount++;

            if (_foldersPerArchivePart.Count > 0)
            {
                if (_foldersPerArchivePart.ContainsKey(_currentArchivePart))
                    _foldersPerArchivePart[_currentArchivePart].Add(folder);
            }
        }

        private class Folder
        {
            public string SystemId { get; set; }
            public string MappeId { get; set; }
            public long LineNumber { get; }
            public string Status { get; set; }
            public bool HasSubFolderOrRegistration { get; set; }

            public Folder(long lineNumber)
            {
                LineNumber = lineNumber;
            }

            public override string ToString()
            {
                return string.Format(Noark5Messages.FolderSystemIdAndFolderId, SystemId, MappeId);
            }
        }
    }
}
