using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #20
    /// </summary>
    public class NumberOfEachDocumentStatus : Noark5XmlReaderBaseTest
    {
        private string _currentArchivePartSystemId;
        private DocumentDescription _currentDocumentDescription;
        private readonly List<DocumentDescription> _documentDescriptions = new List<DocumentDescription>();

        public override string GetName()
        {
            return Noark5Messages.NumberOfEachDocumentStatus;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            var documentDescriptionQuery = from documentDescription in _documentDescriptions
                group documentDescription by new
                {
                    documentDescription.ArchivePartSystemId,
                    documentDescription.Status
                }
                into grouped
                select new
                {
                    grouped.Key.ArchivePartSystemId,
                    grouped.Key.Status,
                    Count = grouped.Count()
                };

            bool multipleArchiveParts = _documentDescriptions.GroupBy(j => j.ArchivePartSystemId).Count() > 1;

            foreach (var item in documentDescriptionQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfEachDocumentStatusMessage, item.Status, item.Count));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.NumberOfEachDocumentStatus_ArchivePartSystemId,
                            item.ArchivePartSystemId) + " - ");

                ResultType resultType =
                    item.Status.Equals("Dokumentet er ferdigstilt")
                        ? ResultType.Success
                        : ResultType.Error;

                testResults.Add(new TestResult(resultType, new Location(""), message.ToString()));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("dokumentbeskrivelse", "registrering"))
                _currentDocumentDescription = new DocumentDescription
                    {ArchivePartSystemId = _currentArchivePartSystemId};
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("dokumentstatus", "dokumentbeskrivelse") && _currentDocumentDescription != null)
                _currentDocumentDescription.Status = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentbeskrivelse") && _currentDocumentDescription != null)
            {
                _documentDescriptions.Add(_currentDocumentDescription);
                _currentDocumentDescription = null;
            }
        }

        private class DocumentDescription
        {
            public string ArchivePartSystemId { get; set; }
            public string Status { get; set; }
        }
    }
}
