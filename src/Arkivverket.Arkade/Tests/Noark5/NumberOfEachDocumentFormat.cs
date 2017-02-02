using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #24
    /// </summary>
    public class NumberOfEachDocumentFormat : Noark5XmlReaderBaseTest
    {
        private string _currentArchivePartSystemId;
        private DocumentObject _currentDocumentObject;
        private readonly List<DocumentObject> _documentObjects = new List<DocumentObject>();

        public override string GetName()
        {
            return Noark5Messages.NumberOfEachDocumentFormat;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            // Separate format-mismatching document objects from the rest:
            var documentObjectsWithFormatMismatch = _documentObjects.FindAll(d => d.HasFormatMismatch());
            _documentObjects.RemoveAll(d => d.HasFormatMismatch());

            // Group document objects by format and by archive part:
            var documentObjectQuery = from documentObject in _documentObjects
                group documentObject by new
                {
                    documentObject.ArchivePartSystemId,
                    documentObject.Format,
                }
                into grouped
                select new
                {
                    grouped.Key.ArchivePartSystemId,
                    grouped.Key.Format,
                    Count = grouped.Count()
                };

            bool multipleArchiveParts = _documentObjects.GroupBy(j => j.ArchivePartSystemId).Count() > 1;

            foreach (var item in documentObjectQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfEachDocumentFormatMessage, item.Format, item.Count));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.NumberOfEachDocumentFormatMessage_ArchivePartSystemId,
                            item.ArchivePartSystemId) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            foreach (var item in documentObjectsWithFormatMismatch)
                testResults.Add(new TestResult(ResultType.Error, new Location(""),
                    string.Format(
                        Noark5Messages.NumberOfEachDocumentFormatMessage_FormatMismatch,
                        item.Format,
                        item.FileReference
                    )));

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

            if (_currentDocumentObject == null)
                return;

            if (eventArgs.Path.Matches("format", "dokumentobjekt"))
                _currentDocumentObject.Format = eventArgs.Value;

            if (eventArgs.Path.Matches("referanseDokumentfil", "dokumentobjekt"))
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
            public string Format { get; set; }
            public string FileReference { get; set; }

            public bool HasFormatMismatch()
            {
                var fileExtension = Path.GetExtension(FileReference);
                return fileExtension != null && !fileExtension.Equals($".{Format}");
            }
        }
    }
}
