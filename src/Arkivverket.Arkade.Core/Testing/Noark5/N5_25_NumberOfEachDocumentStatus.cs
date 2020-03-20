using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_25_NumberOfEachDocumentStatus : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 25);

        private ArchivePart _currentArchivePart = new ArchivePart();
        private DocumentDescription _currentDocumentDescription;
        private readonly List<DocumentDescription> _documentDescriptions = new List<DocumentDescription>();

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

            var documentDescriptionQuery = from documentDescription in _documentDescriptions
                group documentDescription by new
                {
                    documentDescription.ArchivePart.SystemId,
                    documentDescription.ArchivePart.Name,
                    documentDescription.Status
                }
                into grouped
                select new
                {
                    grouped.Key.SystemId,
                    grouped.Key.Name,
                    grouped.Key.Status,
                    Count = grouped.Count()
                };

            bool multipleArchiveParts = _documentDescriptions.GroupBy(j => j.ArchivePart.SystemId).Count() > 1;

            foreach (var item in documentDescriptionQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfEachDocumentStatusMessage, item.Status, item.Count));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, item.SystemId, item.Name) + " - ");

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
                    {ArchivePart = _currentArchivePart};
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

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

            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        private class DocumentDescription
        {
            public ArchivePart ArchivePart { get; set; }
            public string Status { get; set; }
        }

    }
}
