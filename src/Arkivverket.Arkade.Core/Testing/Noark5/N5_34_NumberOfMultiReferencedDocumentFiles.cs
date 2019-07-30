using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_34_NumberOfMultiReferencedDocumentFiles : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 34);

        private readonly List<DocumentObject> _documentObjects = new List<DocumentObject>();
        private string _currentArchivePartSystemId;
        private DocumentObject _currentDocumentObject;

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
            int multiReferencedDocumentFilesCount = 0;

            // Group document objects by file reference and by archive part:
            var documentObjectQuery = from documentObject in _documentObjects
                group documentObject by new
                {
                    documentObject.ArchivePartSystemId,
                    documentObject.FileReference
                }
                into grouped
                select new
                {
                    grouped.Key.ArchivePartSystemId,
                    grouped.Key.FileReference,
                    Count = grouped.Count()
                };

            bool multipleArchiveParts = _documentObjects.GroupBy(j => j.ArchivePartSystemId).Count() > 1;

            foreach (var item in documentObjectQuery)
            {
                if (item.Count == 1)
                    continue;

                var message = new StringBuilder(
                    string.Format(
                        Noark5Messages.NumberOfMultiReferencedDocumentFilesMessage,
                        item.FileReference,
                        item.Count
                    ));

                multiReferencedDocumentFilesCount++;

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(
                            Noark5Messages.ArchivePartSystemId, item.ArchivePartSystemId) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""),
                string.Format(Noark5Messages.TotalResultNumber, multiReferencedDocumentFilesCount.ToString())));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("dokumentobjekt", "dokumentbeskrivelse", "registrering"))
                _currentDocumentObject = new DocumentObject
                    {ArchivePartSystemId = _currentArchivePartSystemId};
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("referanseDokumentfil", "dokumentobjekt") && _currentDocumentObject != null)
                _currentDocumentObject.FileReference = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentobjekt") && _currentDocumentObject != null)
            {
                _documentObjects.Add(_currentDocumentObject);
                _currentDocumentObject = null;
            }
        }

        private class DocumentObject
        {
            public string ArchivePartSystemId { get; set; }
            public string FileReference { get; set; }
        }
    }
}
