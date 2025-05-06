using System.Collections.Generic;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes.Hardcoded
{
    public class AH_02_ControlExtraOrMissingFiles : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.AddmlHardcoded, 2);

        public const string Name = "Control_ExtraOrMissingFiles";

        private readonly AddmlDefinition _addmlDefinition;
        private readonly Archive _archive;

        private readonly List<string> _knownFiles = new List<string> {
            "addml.xml",
            "addml.xsd",
            "NOARKIH.XML"
        };

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Name;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        public override string GetDescription()
        {
            return Messages.ControlExtraOrMissingFilesDescription;
        }

        protected override List<TestResult> GetTestResults()
        {
            HashSet<string> allFilesInWorkingDirectory = GetAllFilesInDirectory(_archive.Content.DirectoryInfo());
            HashSet<string> allFilesInAddml = GetAllFilesInAddmlDefinition(_addmlDefinition);

            allFilesInWorkingDirectory.ExceptWith(_knownFiles);

            HashSet<string> filesInWorkingDirectoryNotInAddml = new HashSet<string>(allFilesInWorkingDirectory.Except(allFilesInAddml));

            IEnumerable<string> filesInAddmlNotInWorkingDirectory = allFilesInAddml.Except(allFilesInWorkingDirectory);
            
            List<TestResult> testResults = new List<TestResult>();
            foreach (string s in filesInWorkingDirectoryNotInAddml)
            {
                testResults.Add(new TestResult(ResultType.Error, new Location(s),
                  string.Format(Messages.ControlExtraOrMissingFilesMessage2)));
            }
            foreach (string s in filesInAddmlNotInWorkingDirectory)
            {
                testResults.Add(new TestResult(ResultType.Error, new Location(s),
                  string.Format(Messages.ControlExtraOrMissingFilesMessage1)));
            }

            return testResults;
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoRun(Field field)
        {
        }

        protected override void DoEndOfFile()
        {
        }

        public AH_02_ControlExtraOrMissingFiles(AddmlDefinition addmlDefinition, Archive archive)
        {
            _addmlDefinition = addmlDefinition;
            _archive = archive;
        }

        private HashSet<string> GetAllFilesInAddmlDefinition(AddmlDefinition addmlDefinition)
        {
            return new HashSet<string>(
                addmlDefinition.AddmlFlatFileDefinitions
                .Select(d => d.FileName.RelativeFilename)
                .AsEnumerable());
        }

        private HashSet<string> GetAllFilesInDirectory(DirectoryInfo directory)
        {
            string[] files = Directory.GetFiles(directory.FullName, "*", SearchOption.AllDirectories);

            return new HashSet<string>(
                files.Select(f => f.Substring(directory.FullName.Length + 1))
                .AsEnumerable());
        }
    }
}