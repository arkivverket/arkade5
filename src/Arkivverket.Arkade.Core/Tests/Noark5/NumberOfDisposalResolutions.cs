using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Tests.Noark5
{
    public class NumberOfDisposalResolutions : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 44);

        private string _currentArchivePartSystemId;
        private bool _multipleArchiveParts;
        private readonly List<DisposalResolution> _disposalResolutions;
        private readonly bool _documentationStatesDisposalResolutions;

        public NumberOfDisposalResolutions(Archive testArchive)
        {
            _disposalResolutions = new List<DisposalResolution>();
            _documentationStatesDisposalResolutions = DocumentationStatesDisposalResolutions(testArchive);
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfDisposalResolutions;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            // Group disposal resolutions by parent element name and by archive part:
            var disposalResolutionQuery = from disposalResolution in _disposalResolutions
                group disposalResolution by new
                {
                    disposalResolution.ArchivePartSystemId,
                    disposalResolution.ParentElementName
                }
                into grouped
                select new
                {
                    grouped.Key.ArchivePartSystemId,
                    grouped.Key.ParentElementName,
                    Count = grouped.Count()
                };

            foreach (var item in disposalResolutionQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfDisposalResolutionsMessage, item.ParentElementName, item.Count));

                if (_multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, item.ArchivePartSystemId) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            // Error message if documentation states instances of disposal resolutions but none are found:
            if (_documentationStatesDisposalResolutions && !_disposalResolutions.Any())
                testResults.Add(new TestResult(ResultType.Error, new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                    Noark5Messages.NumberOfDisposalResolutionsMessage_DocTrueActualFalse));

            // Error message if documentation states no instances of disposal resolutions but some are found:
            if (!_documentationStatesDisposalResolutions && _disposalResolutions.Any())
                testResults.Add(new TestResult(ResultType.Error, new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                    Noark5Messages.NumberOfDisposalResolutionsMessage_DocFalseActualTrue));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("kassasjon"))
            {
                _disposalResolutions.Add(new DisposalResolution
                {
                    ArchivePartSystemId = _currentArchivePartSystemId,
                    ParentElementName = eventArgs.Path.GetParent()
                });
            }
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
            {
                if (_currentArchivePartSystemId != null)
                    _multipleArchiveParts = true;

                _currentArchivePartSystemId = eventArgs.Value;
            }
        }

        private static bool DocumentationStatesDisposalResolutions(Archive archive)
        {
            string archiveExtractionXmlFile =
                archive.WorkingDirectory.Content().WithFile(ArkadeConstants.ArkivuttrekkXmlFileName).FullName;

            var archiveExtractionXml = SerializeUtil.DeserializeFromFile<addml>(archiveExtractionXmlFile);

            dataObject archiveExtractionElement = archiveExtractionXml.dataset[0].dataObjects.dataObject[0];
            property infoElement = archiveExtractionElement.properties[0];
            property additionalInfoElement = infoElement.properties[1];
            property documentCountProperty =
                additionalInfoElement.properties.FirstOrDefault(p => p.name == "inneholderDokumenterSomSkalKasseres");

            return documentCountProperty != null && bool.Parse(documentCountProperty.value);
        }

        private class DisposalResolution
        {
            public string ArchivePartSystemId { get; set; }
            public string ParentElementName { get; set; }
        }
    }
}
