using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_32_ControlDocumentFilesExists : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 32);

        private readonly List<TestResult> _testResults = new List<TestResult>();

        private readonly DirectoryInfo _workingDirectory;

        private readonly Dictionary<ArchivePart, List<string>> _missingFilesPerArchivepart = new Dictionary<ArchivePart, List<string>>();
        private ArchivePart _currentArchivePart = new ArchivePart(); 

        public N5_32_ControlDocumentFilesExists(Archive archive)
        {
            _workingDirectory = archive.WorkingDirectory.Content().DirectoryInfo();
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            foreach (KeyValuePair<ArchivePart, List<string>> missingFilesAtArchivepart in _missingFilesPerArchivepart)
            {
                var message = "";

                if (_missingFilesPerArchivepart.Keys.Count > 1)
                {
                    message = string.Format(Noark5Messages.ArchivePartSystemId, missingFilesAtArchivepart.Key.SystemId, missingFilesAtArchivepart.Key.Name) + " - ";
                }

                foreach (string missingFile in missingFilesAtArchivepart.Value)
                {
                    _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        message + string.Format(Noark5Messages.FileNotFound, missingFile)));
                }
            }
            
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
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("referanseDokumentfil"))
            {
                string documentFileName = eventArgs.Value;

                if (!FileExists(documentFileName))
                {
                    if (_missingFilesPerArchivepart.ContainsKey(_currentArchivePart))
                        _missingFilesPerArchivepart[_currentArchivePart].Add(documentFileName);
                    else
                        _missingFilesPerArchivepart.Add(_currentArchivePart, new List<string>{documentFileName});
                    
                }
            }
        }

        private bool FileExists(string documentFileName)
        {
            if (Path.DirectorySeparatorChar == '/')
                documentFileName = documentFileName.Replace('\\', '/');

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
