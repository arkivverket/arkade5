using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_10_NumberOfFolders : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 10);

        private readonly Archive _archive;
        private readonly Stack<string> _currentFolderType = new();
        private readonly List<N5_10_ArchivePart> _archiveParts = new();
        private N5_10_ArchivePart _currentArchivePart = new();

        public N5_10_NumberOfFolders(Archive archive)
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
            int totalNumberOfFolders = _archiveParts.Sum(a => a.FoldersPerLevel.Sum(f => f.Value.Values.Sum()));
            int documentedNumberOfFolders = GetDocumentedNumberOfFolders();
            bool multipleArchiveParts = _archiveParts.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(""), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfFolders))
                }
            };

            if (totalNumberOfFolders != documentedNumberOfFolders)
                testResultSet.TestsResults.Add(
                    new TestResult(ResultType.Error, new Location(""), string.Format(
                        Noark5Messages.NumberOfFolders_DocumentedAndActualMismatch,
                        documentedNumberOfFolders, totalNumberOfFolders
                    )));

            if (totalNumberOfFolders == 0)
                return testResultSet;

            foreach (N5_10_ArchivePart archivePart in _archiveParts)
            {
                int numberOfFolders = archivePart.FoldersPerLevel.Sum(f => f.Value.Values.Sum());

                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                    TestsResults = new List<TestResult>
                    {
                        new(ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.NumberOf, numberOfFolders))
                    }
                };

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(archivePartResultSet);

                if (numberOfFolders == 0)
                    continue;

                foreach ((string folderName, Dictionary<int, int> numberOfFoldersPerLevel) in archivePart.FoldersPerLevel)
                {
                    string folderType = Noark5TestHelper.StripNamespace(folderName);

                    var folderTypeResultSet = new TestResultSet
                    {
                        Name = string.Format(Noark5Messages.FolderType, folderType),
                        TestsResults = new List<TestResult>
                        {
                            new(ResultType.Success, new Location(string.Empty), string.Format(
                                Noark5Messages.NumberOf, numberOfFoldersPerLevel.Values.Sum()))
                        }
                    };

                    foreach ((int level, int number) in numberOfFoldersPerLevel)
                    {
                        folderTypeResultSet.TestsResults.Add(new TestResult(ResultType.Success,
                            new Location(string.Empty), string.Format(
                                Noark5Messages.NumberOfTypeFoldersAtLevel, level, number)));
                    }

                    if (multipleArchiveParts)
                        archivePartResultSet.TestResultSets.Add(folderTypeResultSet);
                    else
                        testResultSet.TestResultSets.Add(folderTypeResultSet);
                }
            }

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("mappe"))
                _currentFolderType.Push("mappe");
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesTypefolder(eventArgs))
            {
                _currentFolderType.Pop();
                _currentFolderType.Push(eventArgs.Value);
            }
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId =  eventArgs.Value;
            }
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
            {
                _currentArchivePart.Name = eventArgs.Value;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("mappe"))
            {
                int level = eventArgs.Path.GetSameElementSubLevel();
                AddFolderOnLevel(_currentArchivePart.FoldersPerLevel, _currentFolderType.Pop(), level);

            }
            if (eventArgs.NameEquals("arkivdel"))
            {
                _archiveParts.Add(_currentArchivePart);
                _currentArchivePart = new N5_10_ArchivePart();
            }
        }

        private void AddFolderOnLevel(Dictionary<string, Dictionary<int, int>> foldersPerLevel, string name, int level)
        {
            if (foldersPerLevel.ContainsKey(name))
            {
                if (foldersPerLevel[name].ContainsKey(level))
                    foldersPerLevel[name][level]++;
                else foldersPerLevel[name].Add(level, 1);
            }
            else
            {
                foldersPerLevel.Add(name, new Dictionary<int, int>() { [level] = 1 });
            }
        }

        private int GetDocumentedNumberOfFolders()
        {
            addml addml = _archive.AddmlInfo.Addml;

            string numberOfFolders = addml.dataset[0].dataObjects.dataObject[0]
                .dataObjects.dataObject[0].properties.FirstOrDefault(
                    p => p.name == "info")?.properties.Where(
                    p => p.name == "numberOfOccurrences").FirstOrDefault(
                    p => p.value.Equals("mappe"))?.properties.FirstOrDefault(
                    p => p.name.Equals("value"))?
                .value;
            
            return numberOfFolders == null ? 0 : int.Parse(numberOfFolders);
        }

        private class N5_10_ArchivePart : ArchivePart
        {
            public readonly Dictionary<string, Dictionary<int, int>> FoldersPerLevel = new Dictionary<string, Dictionary<int, int>>();
        }
    }
}
