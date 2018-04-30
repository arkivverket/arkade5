using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class ControlDocumentFilesExists : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 32);

        private readonly List<TestResult> _testResults = new List<TestResult>();

        private readonly DirectoryInfo _workingDirectory;

        public ControlDocumentFilesExists(Archive archive)
        {
            _workingDirectory = archive.WorkingDirectory.Content().DirectoryInfo();
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.ControlDocumentFilesExists;
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
                        Noark5Messages.ControlDocumentsFilesExistsMessage1)
                    );
                }
            }
        }

        private bool FileExists(string documentFileName)
        {
            try
            {
                var documentFileInfo = new FileInfo(Path.Combine(_workingDirectory.FullName, documentFileName));

                return documentFileInfo.Exists; // Logging of exists-check greatly affects performance
            }
            catch
            {
                return false; // File reference parse error means file doesn't exist with given filename
            }
        }
    }
}
