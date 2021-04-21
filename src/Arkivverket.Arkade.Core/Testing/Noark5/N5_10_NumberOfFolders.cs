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
        private Stack<string> _currentFolderType = new Stack<string>();
        private readonly List<N5_10_ArchivePart> _archiveParts = new List<N5_10_ArchivePart>();
        private N5_10_ArchivePart _currentArchivePart = new N5_10_ArchivePart();

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

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            var totalNumberOfFolders = 0;
            var message = "";

            foreach (N5_10_ArchivePart archivePart in _archiveParts)
            {
                if (_archiveParts.Count > 1)
                    message = string.Format(Noark5Messages.ArchivePartSystemId, archivePart.SystemId, archivePart.Name) + " - ";

                foreach (var foldersPerLevel in archivePart.FoldersPerLevel)
                { 
                    totalNumberOfFolders += foldersPerLevel.Value.Values.Sum();

                    string folderType = Noark5TestHelper.StripNamespace(foldersPerLevel.Key);

                    testResults.Add(new TestResult(ResultType.Success, new Location(""), string.Format(
                        message + Noark5Messages.NumberOfTypeFolders, folderType, foldersPerLevel.Value.Values.Sum())));

                    if (foldersPerLevel.Value.Count > 1)
                    {
                        foreach (var foldersAtLevel in foldersPerLevel.Value)
                        {
                            testResults.Add(new TestResult(ResultType.Success, new Location(""),
                                message + string.Format(Noark5Messages.NumberOfTypeFoldersAtLevel, folderType,
                                    foldersAtLevel.Key, foldersAtLevel.Value)));
                        }
                    }
                }
            }

            int documentedNumberOfFolders = GetDocumentedNumberOfFolders();

            if (totalNumberOfFolders != documentedNumberOfFolders)
                testResults.Add(new TestResult(ResultType.Error, new Location(""), string.Format(
                    Noark5Messages.NumberOfFolders_DocumentedAndActualMismatch, documentedNumberOfFolders, totalNumberOfFolders)));

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""),
                string.Format(Noark5Messages.TotalResultNumber, totalNumberOfFolders.ToString())));

            return testResults;
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

        private void AddFolderOnLevel(Dictionary<int, int> foldersPerLevel, string value, int level)
        {
            if (foldersPerLevel.ContainsKey(level))
                foldersPerLevel[level]++;

            else foldersPerLevel.Add(level, 1);
        }

        private int GetDocumentedNumberOfFolders()
        {
            addml addml = _archive.AddmlInfo.Addml;

            string numberOfFolders = addml.dataset[0].dataObjects.dataObject[0]
                .dataObjects.dataObject[0].properties.FirstOrDefault(
                    p => p.name == "info").properties.Where(
                    p => p.name == "numberOfOccurrences").FirstOrDefault(
                    p => p.value.Equals("mappe")).properties.FirstOrDefault(
                    p => p.name.Equals("value"))
                .value;

            return int.Parse(numberOfFolders);
        }

        private class N5_10_ArchivePart : ArchivePart
        {
            public readonly Dictionary<string, Dictionary<int, int>> FoldersPerLevel = new Dictionary<string, Dictionary<int, int>>();
        }
    }
}
