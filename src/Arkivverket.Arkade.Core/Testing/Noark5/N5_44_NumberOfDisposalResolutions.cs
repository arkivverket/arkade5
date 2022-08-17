using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_44_NumberOfDisposalResolutions : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 44);

        private readonly Dictionary<ArchivePart, Dictionary<string, int>> _disposalResolutionsPerElementPerArchivePart =
            new();
        private ArchivePart _currentArchivePart = new();
        private readonly bool _documentationStatesDisposalResolutions;
        private readonly List<long> _disposalResolutionLocations = new();
        private int _totalNumberOfDisposalResolutions;

        public N5_44_NumberOfDisposalResolutions(Archive testArchive)
        {
            _documentationStatesDisposalResolutions = DocumentationStatesDisposalResolutions(testArchive);
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
            bool multipleArchiveParts = _disposalResolutionsPerElementPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _totalNumberOfDisposalResolutions))
                }
            };

            foreach ((ArchivePart archivePart, Dictionary<string, int> disposalResolutionsPerElement) in
                _disposalResolutionsPerElementPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((string parentElementName, int numberOfDisposalResolutions) in disposalResolutionsPerElement)
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                        string.Format(Noark5Messages.NumberOfDisposalResolutionsMessage, parentElementName,
                            numberOfDisposalResolutions)));

                if (multipleArchiveParts)
                {
                    testResults.Insert(0, new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOf, disposalResolutionsPerElement.Values.Sum())));

                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                }
                else
                    testResultSet.TestsResults.AddRange(testResults);
            }

            switch (_documentationStatesDisposalResolutions)
            {
                // Error message if documentation states instances of disposal resolutions but none are found:
                case true when _totalNumberOfDisposalResolutions == 0:
                    testResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                        new Location(ArkadeConstants.ArkivstrukturXmlFileName),
                        Noark5Messages.NumberOfDisposalResolutionsMessage_DocTrueActualFalse));
                    break;
                // Error message if documentation states no instances of disposal resolutions but some are found:
                case false when _totalNumberOfDisposalResolutions > 0:
                    testResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                        new Location(ArkadeConstants.ArkivstrukturXmlFileName, _disposalResolutionLocations),
                        Noark5Messages.NumberOfDisposalResolutionsMessage_DocFalseActualTrue));
                    break;
            }

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("kassasjon"))
            {
                string parentElementName = eventArgs.Path.GetParent();

                if (_disposalResolutionsPerElementPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_disposalResolutionsPerElementPerArchivePart[_currentArchivePart].ContainsKey(parentElementName))
                        _disposalResolutionsPerElementPerArchivePart[_currentArchivePart][parentElementName]++;
                    else
                        _disposalResolutionsPerElementPerArchivePart[_currentArchivePart].Add(parentElementName, 1);
                }
                else
                    _disposalResolutionsPerElementPerArchivePart.Add(
                        _currentArchivePart, new Dictionary<string, int>
                        {
                            {parentElementName, 1}
                        }
                    );

                _disposalResolutionLocations.Add(eventArgs.LineNumber);
                _totalNumberOfDisposalResolutions++;
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
            
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        private static bool DocumentationStatesDisposalResolutions(Archive archive)
        {
            addml archiveExtractionXml = archive.AddmlInfo.Addml;

            dataObject archiveExtractionElement = archiveExtractionXml.dataset[0].dataObjects.dataObject[0];
            property infoElement = archiveExtractionElement.properties[0];
            property additionalInfoElement = infoElement.properties[1];
            property documentCountProperty =
                additionalInfoElement.properties.FirstOrDefault(p => p.name == "inneholderDokumenterSomSkalKasseres");

            return documentCountProperty != null && bool.Parse(documentCountProperty.value);
        }
    }
}
