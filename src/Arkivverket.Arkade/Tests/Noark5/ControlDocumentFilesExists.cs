using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #46
    /// </summary>
    public class ControlDocumentFilesExists : Noark5XmlReaderBaseTest
    {
        private readonly List<TestResult> _testResults = new List<TestResult>();

        private readonly DirectoryInfo _workingDirectory;

        public ControlDocumentFilesExists(Archive archive)
        {
            _workingDirectory = archive.WorkingDirectory.Content().DirectoryInfo();
        }

        public override string GetName()
        {
            return Noark5Messages.ControlDocumentFilesExists;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }


        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("referanseDokumentfil"))
            {
                string documentFileName = eventArgs.Value;
                if (!FileExists(documentFileName))
                {
                    _testResults.Add(new TestResult(ResultType.Error, new Location(documentFileName),
                        Noark5Messages.ControlDocumentsFilesExistsMessage1));
                }
            }
        }

        /// <summary>
        ///     Checks if file exists on disk.
        ///     IMPORTANT - DO NOT PERFORM LOGGING ON EACH FILE EXISTS CHECK - IT KILLS PERFORMANCE!
        /// </summary>
        /// <param name="documentFileName"></param>
        /// <returns></returns>
        private bool FileExists(string documentFileName)
        {
            try
            {
                var file = new FileInfo(Path.Combine(_workingDirectory.FullName, documentFileName));
                bool fileExists = file.Exists;
                return fileExists;
            }
            catch
            {
                return false; // File reference parse error means file doesn't exist with given filename
            }
        }
    }
}