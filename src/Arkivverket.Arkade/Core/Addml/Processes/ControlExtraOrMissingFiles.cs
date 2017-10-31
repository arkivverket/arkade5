using System.Collections.Generic;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using System.Diagnostics;
using Arkivverket.Arkade.Core.Addml.Definitions;
using System.IO;
using System.Linq;
using System.Xml;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlExtraOrMissingFiles : IAddmlHardcodedProcess
    {
        public const string Name = "Control_ExtraOrMissingFiles";

        private readonly AddmlDefinition _addmlDefinition;
        private readonly Archive _archive;

        private List<string> _knownFiles = new List<string> {
            "addml.xml",
            "NOARKIH.XML"
        };


        public TestRun GetTestRun()
        {
            Stopwatch _stopwatch = new Stopwatch();

            _stopwatch.Start();
            List<string> allFilesInWorkingDirectory = GetAllFilesInDirectory(_archive.WorkingDirectory.Content().DirectoryInfo());
            allFilesInWorkingDirectory = FilterKnownFiles(allFilesInWorkingDirectory);
            List<string> allFilesInAddml = GetAllFilesInAddmlDefinition(_addmlDefinition);

            List<string> filesInWorkingDirectoryNotInAddml = allFilesInWorkingDirectory.Except(allFilesInAddml).ToList();
            List<string> filesInAddmlNotInWorkingDirectory = allFilesInAddml.Except(allFilesInWorkingDirectory).ToList();

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
            _stopwatch.Stop();

            TestRun testRun = new TestRun(Name, TestType.ContentControl);
            testRun.TestDuration = _stopwatch.ElapsedMilliseconds;
            testRun.TestDescription = Messages.ControlExtraOrMissingFilesDescription;
            testRun.Results = testResults;

            return testRun;
        }

        
        // This can be sensitive for file paths and use of different slashes for file separators
        private void FilterNoark4DokversReferences(List<string> unfiltered)
        {

            var pathToDokvers = _archive.WorkingDirectory.Content().WithSubDirectory("DATA").WithFile("DOKVERS.XML").FullName;
            var xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null; // skip resolving of DTD
            xmlDoc.Load(File.OpenRead(pathToDokvers));
            
            foreach(XmlNode row in xmlDoc.SelectNodes("//VE.FILREF"))
            {
                string dokversReference = row.InnerText;
                
                var fileReference = "DATA";
                if (!dokversReference.StartsWith("\\"))
                    fileReference = fileReference + "\\";
                fileReference = fileReference + dokversReference;
                
                unfiltered.Remove(fileReference);
            }
        }


        public ControlExtraOrMissingFiles(AddmlDefinition addmlDefinition, Archive archive)
        {
            _addmlDefinition = addmlDefinition;
            _archive = archive;
        }

        private List<string> GetAllFilesInAddmlDefinition(AddmlDefinition addmlDefinition)
        {
            return addmlDefinition.AddmlFlatFileDefinitions
                .Select(d => d.FileName)
                .ToList();
        }

        private List<string> GetAllFilesInDirectory(DirectoryInfo directory)
        {
            string[] files = System.IO.Directory.GetFiles(directory.FullName, "*", SearchOption.AllDirectories);

            return files
                .Select(f => f.Substring(directory.FullName.Length + 1))
                .ToList();
        }

        private List<string> FilterKnownFiles(List<string> files)
        {
            return files.Where(f => !_knownFiles.Contains(f)).ToList();
        }
    }
}