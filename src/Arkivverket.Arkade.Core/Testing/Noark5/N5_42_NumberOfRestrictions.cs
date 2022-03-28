using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_42_NumberOfRestrictions : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 42);

        private readonly Dictionary<ArchivePart, Dictionary<string, int>>
            _numberOfRestrictionsPerElementPerArchivePart = new();
        private ArchivePart _currentArchivePart = new();
        private readonly bool _documentationStatesRestrictions;
        private readonly List<long> _restrictionLocations = new();
        private int _totalNumberOfRestrictions;

        public N5_42_NumberOfRestrictions(Archive testArchive)
        {
            _documentationStatesRestrictions = DocumentationStatesRestrictions(testArchive);
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
            bool multipleArchiveParts = _numberOfRestrictionsPerElementPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _totalNumberOfRestrictions))
                }
            };

            switch (_documentationStatesRestrictions)
            {
                // Error message if documentation states instances of restrictions but none are found:
                case true when _totalNumberOfRestrictions == 0:
                    testResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                        new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                        Noark5Messages.NumberOfRestrictionsMessage_DocTrueActualFalse));
                    break;
                // Error message if documentation states no instances of restrictions but some are found:
                case false when _totalNumberOfRestrictions > 0:
                    testResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                        new Location(ArkadeConstants.ArkivuttrekkXmlFileName, _restrictionLocations),
                        Noark5Messages.NumberOfRestrictionsMessage_DocFalseActualTrue));
                    break;
            }

            if (_totalNumberOfRestrictions == 0)
                return testResultSet;

            foreach ((ArchivePart archivePart, Dictionary<string, int> restrictionsPerElement) in
                _numberOfRestrictionsPerElementPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((string parentElementName, int numberOfRestrictions) in restrictionsPerElement)
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfRestrictionsMessage, parentElementName, numberOfRestrictions)));

                if (multipleArchiveParts)
                {
                    testResults.Insert(0, new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOf, restrictionsPerElement.Values.Sum())));

                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                }
                else
                    testResultSet.TestsResults.AddRange(testResults);
            }

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("skjerming"))
            {
                string parentElementName = eventArgs.Path.GetParent();

                if (_numberOfRestrictionsPerElementPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_numberOfRestrictionsPerElementPerArchivePart[_currentArchivePart].ContainsKey(parentElementName))
                        _numberOfRestrictionsPerElementPerArchivePart[_currentArchivePart][parentElementName]++;
                    else
                        _numberOfRestrictionsPerElementPerArchivePart[_currentArchivePart].Add(parentElementName, 1);
                }
                else
                {
                    _numberOfRestrictionsPerElementPerArchivePart.Add(_currentArchivePart,
                        new Dictionary<string, int> {{parentElementName, 1}});
                }

                _restrictionLocations.Add(eventArgs.LineNumber);
                _totalNumberOfRestrictions++;
            }
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
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        private static bool DocumentationStatesRestrictions(Archive archive)
        {
            addml archiveExtractionXml = archive.AddmlInfo.Addml;

            dataObject archiveExtractionElement = archiveExtractionXml.dataset[0].dataObjects.dataObject[0];
            property infoElement = archiveExtractionElement.properties[0];
            property additionalInfoElement = infoElement.properties[1];
            property documentCountProperty =
                additionalInfoElement.properties.FirstOrDefault(p => p.name == "inneholderSkjermetInformasjon");

            return documentCountProperty != null && bool.Parse(documentCountProperty.value);
        }
    }
}
