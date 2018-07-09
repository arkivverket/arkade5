using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Tests;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using System.IO;
using System.Linq;
using System.Xml;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes.Hardcoded
{
    public class ControlExtraOrMissingFiles : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.AddmlHardcoded, 2);

        public const string Name = "Control_ExtraOrMissingFiles";

        private readonly AddmlDefinition _addmlDefinition;
        private readonly Archive _archive;

        private readonly List<string> _knownFiles = new List<string> {
            "addml.xml",
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
            HashSet<string> allFilesInWorkingDirectory = GetAllFilesInDirectory(_archive.WorkingDirectory.Content().DirectoryInfo());
            HashSet<string> allFilesInAddml = GetAllFilesInAddmlDefinition(_addmlDefinition);

            allFilesInWorkingDirectory.ExceptWith(_knownFiles);

            HashSet<string> filesInWorkingDirectoryNotInAddml = new HashSet<string>(allFilesInWorkingDirectory.Except(allFilesInAddml));

            IEnumerable<string> filesInAddmlNotInWorkingDirectory = allFilesInAddml.Except(allFilesInWorkingDirectory);

            if (_archive.ArchiveType.Equals(ArchiveType.Noark4))
            {
                FilterNoark4DokversReferences(filesInWorkingDirectoryNotInAddml);
            }
            
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

        private void FilterNoark4DokversReferences(HashSet<string> unfiltered)
        {
            if (unfiltered == null || !unfiltered.Any())
                return;

            var pathToDokvers = _archive.WorkingDirectory.Content().WithSubDirectory("DATA").WithFile("DOKVERS.XML").FullName;

            // potential spot for using stream reader instead. 
            var xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null; // skip resolving of DTD
            xmlDoc.Load(File.OpenRead(pathToDokvers));

            bool usesForwardSlashAsSeparator = HasForwardSlashInFilename(unfiltered);
            
            foreach (XmlNode row in xmlDoc.SelectNodes("//VE.FILREF"))
            {
                string dokversReference = row.InnerText;

                if (dokversReference.Contains("/") && usesForwardSlashAsSeparator == false)
                    dokversReference = dokversReference.Replace("/", "\\");
                else if (dokversReference.Contains("\\") && usesForwardSlashAsSeparator)
                    dokversReference = dokversReference.Replace("\\", "/");

                var fileReference = "DATA";
                if (!dokversReference.StartsWith("\\"))
                    fileReference = fileReference + "\\";
                fileReference = fileReference + dokversReference;

                unfiltered.Remove(fileReference);
            }
        }

        private bool HasForwardSlashInFilename(HashSet<string> unfiltered)
        {
            int counter = 0;
            foreach(string item in unfiltered)
            {
                if (item.IndexOf("/", StringComparison.Ordinal) != -1)
                    return true;

                if (counter++ == 10) // only check the first 10 entries
                    break;
            }
            return false;
        }

        public ControlExtraOrMissingFiles(AddmlDefinition addmlDefinition, Archive archive)
        {
            _addmlDefinition = addmlDefinition;
            _archive = archive;
        }

        private HashSet<string> GetAllFilesInAddmlDefinition(AddmlDefinition addmlDefinition)
        {
            return new HashSet<string>(
                addmlDefinition.AddmlFlatFileDefinitions
                .Select(d => d.FileName)
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