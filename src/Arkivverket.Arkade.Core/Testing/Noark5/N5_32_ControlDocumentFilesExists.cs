using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_32_ControlDocumentFilesExists : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 32);

        private readonly ReadOnlyDictionary<string, DocumentFile> _documentFiles;

        private readonly Dictionary<ArchivePart, List<(string, long)>> _missingFilesPerArchivePart = new();
        private ArchivePart _currentArchivePart = new(); 

        public N5_32_ControlDocumentFilesExists(Archive archive)
        {
            _documentFiles = archive.DocumentFiles;
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override TestResultSet GetTestResults()
        {
            bool multipleArchiveParts = _missingFilesPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();

            foreach ((ArchivePart archivePart, List<(string, long)> missingFiles) in _missingFilesPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((string missingFile, long xmlLineNumber) in missingFiles)
                {
                    testResults.Add(new TestResult(ResultType.Error, 
                        new Location(ArkadeConstants.ArkivstrukturXmlFileName, xmlLineNumber),
                        string.Format(Noark5Messages.FileNotFound, missingFile)));
                }

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                else
                    testResultSet.TestsResults = testResults;
            }
            
            return testResultSet;
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
                long xmlLineNumber = eventArgs.LineNumber;

                if (!DocumentFileExists(documentFileName))
                {
                    if (_missingFilesPerArchivePart.ContainsKey(_currentArchivePart))
                        _missingFilesPerArchivePart[_currentArchivePart].Add((documentFileName, xmlLineNumber));
                    else
                        _missingFilesPerArchivePart.Add(_currentArchivePart,
                            new List<(string, long)> {(documentFileName, xmlLineNumber)});
                }
            }
        }

        private bool DocumentFileExists(string documentFileName)
        {
            return _documentFiles.ContainsKey(documentFileName.Replace('\\', '/'));
        }
    }
}
