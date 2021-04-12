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
    public class N5_42_NumberOfRestrictions : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 42);

        private ArchivePart _currentArchivePart = new ArchivePart();
        private List<ArchivePart> archiveParts = new List<ArchivePart>();
        private readonly List<Restriction> _restrictions;
        private readonly bool _documentationStatesRestrictions;

        public N5_42_NumberOfRestrictions(Archive testArchive)
        {
            _restrictions = new List<Restriction>();
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

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();
            int totalNumberOfRestrictions = 0;

            // Group restrictions by parent element name and by archive part:
            var restrictionQuery = from restriction in _restrictions
                group restriction by new
                {
                    restriction.ArchivePart.SystemId,
                    restriction.ArchivePart.Name,
                    restriction.ParentElementName
                }
                into grouped
                select new
                {
                    grouped.Key.SystemId,
                    grouped.Key.Name,
                    grouped.Key.ParentElementName,
                    Count = grouped.Count()
                };

            foreach (var item in restrictionQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfRestrictionsMessage, item.ParentElementName, item.Count));

                if (archiveParts.Count > 1)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, item.SystemId, item.Name) + " - ");

                totalNumberOfRestrictions += item.Count;

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            // Error message if documentation states instances of restrictions but none are found:
            if (_documentationStatesRestrictions && !_restrictions.Any())
                testResults.Add(new TestResult(ResultType.Error, new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                    Noark5Messages.NumberOfRestrictionsMessage_DocTrueActualFalse));

            // Error message if documentation states no instances of restrictions but some are found:
            if (!_documentationStatesRestrictions && _restrictions.Any())
                testResults.Add(new TestResult(ResultType.Error, new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                    Noark5Messages.NumberOfRestrictionsMessage_DocFalseActualTrue));

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""), 
                string.Format(Noark5Messages.TotalResultNumber, totalNumberOfRestrictions.ToString())));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("skjerming"))
            {
                _restrictions.Add(new Restriction
                {
                    ArchivePart = _currentArchivePart,
                    ParentElementName = eventArgs.Path.GetParent()
                });
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
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                archiveParts.Add(_currentArchivePart);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        private static bool DocumentationStatesRestrictions(Archive archive)
        {
            var archiveExtractionXml = SerializeUtil.DeserializeFromFile<addml>(archive.AddmlXmlUnit.File);

            dataObject archiveExtractionElement = archiveExtractionXml.dataset[0].dataObjects.dataObject[0];
            property infoElement = archiveExtractionElement.properties[0];
            property additionalInfoElement = infoElement.properties[1];
            property documentCountProperty =
                additionalInfoElement.properties.FirstOrDefault(p => p.name == "inneholderSkjermetInformasjon");

            return documentCountProperty != null && bool.Parse(documentCountProperty.value);
        }

        private class Restriction
        {
            public ArchivePart ArchivePart { get; set; }
            public string ParentElementName { get; set; }
        }

    }
}
