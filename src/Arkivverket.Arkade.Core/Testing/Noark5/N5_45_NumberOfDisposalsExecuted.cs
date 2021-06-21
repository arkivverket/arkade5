using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_45_NumberOfDisposalsExecuted : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 45);

        private readonly Dictionary<ArchivePart, int> _numberOfDisposalsExecutedPerArchivePart;
        private ArchivePart _currentArchivePart = new ArchivePart();
        private readonly bool _disposalsAreDocumented;

        public N5_45_NumberOfDisposalsExecuted(Archive archive)
        {
            _numberOfDisposalsExecutedPerArchivePart = new Dictionary<ArchivePart, int>();
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

        protected override TestResultSet GetTestResults()
        {
            bool multipleArchiveParts = _numberOfDisposalsExecutedPerArchivePart.Count > 1;

            int totalNumberOfDisposalsExecuted = _numberOfDisposalsExecutedPerArchivePart.Sum(a => a.Value);

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfDisposalsExecuted))
                }
            };

            switch (_disposalsAreDocumented)
            {
                // Error message if disposals are documented but not found:
                case true when !_numberOfDisposalsExecutedPerArchivePart.Any(a => a.Value > 0):
                    testResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                        new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                        Noark5Messages.NumberOfDisposalsExecutedMessage_DocTrueActualFalse));
                    break;
                // Error message if disposals are found but not documented:
                case false when _numberOfDisposalsExecutedPerArchivePart.Any(a => a.Value > 0):
                    testResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                        new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                        Noark5Messages.NumberOfDisposalsExecutedMessage_DocFalseActualTrue));
                    break;
            }

            if (!multipleArchiveParts)
                return testResultSet;

            foreach ((ArchivePart archivePart, int archivePartDisposalsCount)
                in _numberOfDisposalsExecutedPerArchivePart)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfXPerY, archivePart, archivePartDisposalsCount)));

            return testResultSet;
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
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                _numberOfDisposalsExecutedPerArchivePart.Add(_currentArchivePart, 0);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        private static bool DisposalsAreDocumented(Archive archive)
        {
            addml archiveExtractionXml = archive.AddmlInfo.Addml;

            dataObject archiveExtractionElement = archiveExtractionXml.dataset[0].dataObjects.dataObject[0];
            property infoElement = archiveExtractionElement.properties[0];
            property additionalInfoElement = infoElement.properties[1];
            property documentCountProperty =
                additionalInfoElement.properties.FirstOrDefault(p => p.name == "omfatterDokumenterSomErKassert");

            return documentCountProperty != null && bool.Parse(documentCountProperty.value);
        }
    }
}
