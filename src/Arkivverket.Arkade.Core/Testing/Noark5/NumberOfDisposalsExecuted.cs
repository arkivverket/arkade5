using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class NumberOfDisposalsExecuted : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 45);

        private readonly Dictionary<string, int> _numberOfDisposalsExecutedPerArchivePart;
        private readonly bool _disposalsAreDocumented;

        public NumberOfDisposalsExecuted(Archive archive)
        {
            _numberOfDisposalsExecutedPerArchivePart = new Dictionary<string, int>();
            _disposalsAreDocumented = DisposalsAreDocumented(archive);
        }

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

            foreach (var archivePartDisposalsCount in _numberOfDisposalsExecutedPerArchivePart)
            {
                if (archivePartDisposalsCount.Value == 0)
                    continue;

                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfDisposalsExecutedMessage, archivePartDisposalsCount.Value)
                );

                if (_numberOfDisposalsExecutedPerArchivePart.Keys.Count > 1) // Multiple archiveparts
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, archivePartDisposalsCount.Key) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            // Error message if disposals are documented but not found:
            if (_disposalsAreDocumented && !_numberOfDisposalsExecutedPerArchivePart.Any(a => a.Value > 0))
                testResults.Add(new TestResult(ResultType.Error, new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                    Noark5Messages.NumberOfDisposalsExecutedMessage_DocTrueActualFalse));

            // Error message if disposals are found but not documented:
            if (!_disposalsAreDocumented && _numberOfDisposalsExecutedPerArchivePart.Any(a => a.Value > 0))
                testResults.Add(new TestResult(ResultType.Error, new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                    Noark5Messages.NumberOfDisposalsExecutedMessage_DocFalseActualTrue));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("utfoertKassasjon", "arkivdel") ||
                eventArgs.Path.Matches("utfoertKassasjon", "dokumentbeskrivelse"))
                _numberOfDisposalsExecutedPerArchivePart[_numberOfDisposalsExecutedPerArchivePart.Keys.Last()]++;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _numberOfDisposalsExecutedPerArchivePart[eventArgs.Value] = 0;
        }

        private static bool DisposalsAreDocumented(Archive archive)
        {
            var archiveExtractionXml = SerializeUtil.DeserializeFromFile<addml>(archive.AddmlXmlUnit.File);

            dataObject archiveExtractionElement = archiveExtractionXml.dataset[0].dataObjects.dataObject[0];
            property infoElement = archiveExtractionElement.properties[0];
            property additionalInfoElement = infoElement.properties[1];
            property documentCountProperty =
                additionalInfoElement.properties.FirstOrDefault(p => p.name == "omfatterDokumenterSomErKassert");

            return documentCountProperty != null && bool.Parse(documentCountProperty.value);
        }
    }
}
