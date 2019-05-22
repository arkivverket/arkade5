using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class NumberOfFolders : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 10);

        private readonly Archive _archive;
        private readonly List<ArchivePart> _archiveParts = new List<ArchivePart>();
        private ArchivePart _currentArchivePart;

        public NumberOfFolders(Archive archive)
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

            foreach (ArchivePart archivePart in _archiveParts)
            {
                if (_archiveParts.Count > 1)
                    message = string.Format(Noark5Messages.ArchivePartSystemId, archivePart.SystemId) + " - ";

                foreach (var foldersPerLevel in archivePart.FoldersPerLevel)
                { 
                    totalNumberOfFolders += foldersPerLevel.Value.Values.Sum();

                    string folderType = StripNamespace(foldersPerLevel.Key);

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

            return testResults;
        }

        private string StripNamespace(string folder)
        {
            int index = folder.IndexOf(":");
            if (index >= 0)
            {
                return folder.Substring(index + 1);
            }
            return folder;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesTypefolder(eventArgs))
            {
                int level = eventArgs.Path.GetSameElementSubLevel();
                AddFolderOnLevel(_currentArchivePart.FoldersPerLevel, eventArgs.Value, level);
            }
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart = new ArchivePart {SystemId = eventArgs.Value};
                _archiveParts.Add(_currentArchivePart);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
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
            addml addml = SerializeUtil.DeserializeFromFile<addml>(_archive.AddmlXmlUnit.File);

            string numberOfFolders = addml.dataset[0].dataObjects.dataObject[0]
                .dataObjects.dataObject[0].properties.FirstOrDefault(
                    p => p.name == "info").properties.Where(
                    p => p.name == "numberOfOccurrences").FirstOrDefault(
                    p => p.value.Equals("mappe")).properties.FirstOrDefault(
                    p => p.name.Equals("value"))
                .value;

            return int.Parse(numberOfFolders);
        }

        private class ArchivePart
        {
            public string SystemId { get; set; }
            public readonly Dictionary<string, Dictionary<int, int>> FoldersPerLevel = new Dictionary<string, Dictionary<int, int>>();
        }
    }
}
