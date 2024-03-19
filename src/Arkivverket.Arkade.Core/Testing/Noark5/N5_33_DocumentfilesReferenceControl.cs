using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_33_DocumentfilesReferenceControl : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 33);

        private static Dictionary<string, DocumentFile> _documentFileNames;
        private static string _documentsDirectoryName;

        public N5_33_DocumentfilesReferenceControl(Archive archive)
        {
            _documentsDirectoryName = archive.GetDocumentsDirectoryName();
            _documentFileNames = new Dictionary<string, DocumentFile>(archive.DocumentFiles.Get());
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override TestResultSet GetTestResults()
        {
            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _documentFileNames.Count))
                }
            };

            foreach ((string fileName, DocumentFile _) in _documentFileNames)
            {
                testResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                    new Location(_documentsDirectoryName),
                    string.Format(Noark5Messages.DocumentfilesReferenceControlMessage, fileName)));
            }

            return testResultSet;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("referanseDokumentfil", "dokumentobjekt"))
                _documentFileNames.Remove(eventArgs.Value.Replace("\\", "/"));
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
