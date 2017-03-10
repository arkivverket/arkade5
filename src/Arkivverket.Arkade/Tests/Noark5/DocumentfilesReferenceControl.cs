using System.Collections.Generic;
using System.Collections;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #47
    /// </summary>
    public class DocumentfilesReferenceControl : Noark5XmlReaderBaseTest
    {
        private static FileInfo[] _documentFilesInfo;
        private readonly Hashtable _fileReferences;

        public DocumentfilesReferenceControl(Archive archive)
        {
            _documentFilesInfo = GetNamesActualFiles(archive);
            _fileReferences = new Hashtable();
        }

        public override string GetName()
        {
            return Noark5Messages.DocumentfilesReferenceControl;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            foreach (FileInfo documentFileInfo in _documentFilesInfo)
            {
                if (!_fileReferences.Contains(documentFileInfo.Name))
                {
                    testResults.Add(new TestResult(ResultType.Error,
                        new Location(ArkadeConstants.DirectoryNameDocuments),
                        string.Format(Noark5Messages.DocumentfilesReferenceControlMessage, documentFileInfo.Name)));
                }
            }

            return testResults;
        }

        private static FileInfo[] GetNamesActualFiles(Archive archive)
        {
            return archive.WorkingDirectory.Content()
                .WithSubDirectory(ArkadeConstants.DirectoryNameDocuments)
                .DirectoryInfo().GetFiles();
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("referanseDokumentfil", "dokumentobjekt"))
                _fileReferences.Add(new FileInfo(eventArgs.Value).Name, null);
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
    }
}
