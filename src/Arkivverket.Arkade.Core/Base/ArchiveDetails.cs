using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveDetails : IArchiveDetails
    {
        private string _archiveStandard;

        public string ArchiveCreators => GetArchiveCreators();
        public string ArchivalPeriod => GetArchivalPeriod();
        public string SystemName => GetSystemName();
        public string SystemType => GetSystemType();
        public virtual string ArchiveStandard => _archiveStandard?? GetArchiveStandardVersion();
        public Dictionary<string, IEnumerable<string>> DocumentedXmlUnits => GetDocumentedXmlUnits();
        public Dictionary<string, IEnumerable<string>> StandardXmlUnits => GetStandardXmlUnits();

        private readonly addml _addml;

        public ArchiveDetails(addml addml)
        {
            _addml = addml;
        }

        private string GetArchiveCreators()
        {
            if (!TryGetContextAdditionalElementsRoot(out additionalElements additionalElementsRoot))
                return string.Empty;

            IEnumerable<additionalElement> archiveCreators = GetAdditionalElements(
                additionalElementsRoot, "recordCreator");

            return string.Join(", ", archiveCreators.Select(a => a.value));
        }

        private string GetArchivalPeriod()
        {
            if (!TryGetContentAdditionalElementsRoot(out additionalElements additionalElementsRoot))
                    return string.Empty;

            additionalElement archivalPeriodElement = GetAdditionalElements(
                additionalElementsRoot, "archivalPeriod"
            ).FirstOrDefault();

            property startDate = archivalPeriodElement?.properties.FirstOrDefault(p => p.name.Equals("startDate"));
            property endDate = archivalPeriodElement?.properties.FirstOrDefault(p => p.name.Equals("endDate"));

            return $"{startDate?.value ?? string.Empty} - {endDate?.value ?? string.Empty}";
        }

        private string GetSystemName()
        {
            if (!TryGetContextAdditionalElementsRoot(out additionalElements additionalElementsRoot))
                return string.Empty;
            
            return GetAdditionalElements(additionalElementsRoot, "systemName").FirstOrDefault()?.value ?? string.Empty;
        }

        private string GetSystemType()
        {
            if (!TryGetContextAdditionalElementsRoot(out additionalElements additionalElementsRoot))
                return string.Empty;

            return GetAdditionalElements(additionalElementsRoot, "systemType").FirstOrDefault()?.value ?? string.Empty;
        }

        private string GetArchiveStandardVersion()
        {
            try
            {
                dataObject archiveExtractionElement = _addml.dataset[0].dataObjects.dataObject[0];

                string archiveExtractionTypeVersion = archiveExtractionElement.properties
                    .FirstOrDefault(property => property.name == "info")?.properties
                    .FirstOrDefault(property => property.name == "type")?.properties
                    .FirstOrDefault(property => property.name == "version")?.value;

                _archiveStandard = archiveExtractionTypeVersion;
                return archiveExtractionTypeVersion;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private Dictionary<string, IEnumerable<string>> GetDocumentedXmlUnits()
        {
            var documentedXmlUnits = new Dictionary<string, IEnumerable<string>>();

            try
            {
                dataObject archiveExtractionElement = _addml.dataset[0].dataObjects.dataObject[0];
                List<dataObject> fileElements = archiveExtractionElement.dataObjects.dataObject.ToList();

                foreach (dataObject fileElement in fileElements)
                {
                    string xmlFileName = fileElement.properties
                        .FirstOrDefault(property => property.name == "file")?.properties
                        .FirstOrDefault(property => property.name == "name")?.value;

                    if (string.IsNullOrEmpty(xmlFileName))
                        continue;

                    IEnumerable<property> schemas = fileElement.properties.Where(property => property.name == "schema");

                    var xmlSchemaFileNames = new List<string>();

                    foreach (property schema in schemas)
                    {
                        string documentedXmlSchemaFileName = schema.properties
                            .FirstOrDefault(property => property.name == "file")?.properties
                            .FirstOrDefault(property => property.name == "name")?.value;

                        if (string.IsNullOrEmpty(documentedXmlSchemaFileName))
                            continue;

                        xmlSchemaFileNames.Add(documentedXmlSchemaFileName);
                    }

                    documentedXmlUnits.Add(xmlFileName, xmlSchemaFileNames);
                }
            }
            catch (Exception exception)
            {
                throw new ArkadeException($"Error reading addml: {exception.Message}");
            }

            return documentedXmlUnits;
        }

        private static Dictionary<string, IEnumerable<string>> GetStandardXmlUnits()
        {
            var standardXmlUnits = new Dictionary<string, IEnumerable<string>>
            {
                {
                    ArkadeConstants.ArkivuttrekkXmlFileName, 
                    new[]
                    {
                        ArkadeConstants.AddmlXsdFileName,
                    }
                },
                {
                    ArkadeConstants.ArkivstrukturXmlFileName,
                    new[]
                    {
                        ArkadeConstants.ArkivstrukturXsdFileName,
                        ArkadeConstants.MetadatakatalogXsdFileName,
                    }
                },
                {
                    ArkadeConstants.ChangeLogXmlFileName,
                    new[]
                    {
                        ArkadeConstants.ChangeLogXsdFileName,
                        ArkadeConstants.MetadatakatalogXsdFileName,
                    }
                },
                {
                    ArkadeConstants.RunningJournalXmlFileName,
                    new[]
                    {
                        ArkadeConstants.RunningJournalXsdFileName,
                        ArkadeConstants.MetadatakatalogXsdFileName,
                    }
                },
                {
                    ArkadeConstants.PublicJournalXmlFileName,
                    new[]
                    {
                        ArkadeConstants.PublicJournalXsdFileName,
                        ArkadeConstants.MetadatakatalogXsdFileName,
                    }
                },
            };

            return standardXmlUnits;
        }

        private bool TryGetContentAdditionalElementsRoot(out additionalElements additionalElements)
        {
            try
            {
                additionalElements = _addml.dataset[0].reference.content.additionalElements;
                return true;
            }
            catch
            {
                additionalElements = null;
            }

            return false;
        }

        private bool TryGetContextAdditionalElementsRoot(out additionalElements additionalElements)
        {
            try
            {
                additionalElements = _addml.dataset[0].reference.context.additionalElements;
                return true;
            }
            catch
            {
                additionalElements = null;
            }

            return false;
        }

        private static IEnumerable<additionalElement> GetAdditionalElements(additionalElements root, string name)
        {
            var additionalElements = new List<additionalElement>();

            foreach (additionalElement child in root.additionalElement)
            {
                if (child.name.Equals(name))
                    additionalElements.Add(child);

                else if (child.additionalElements != null && child.additionalElements.additionalElement?.Length > 0)
                    additionalElements.AddRange(GetAdditionalElements(child.additionalElements, name));
            }

            return additionalElements;
        }
    }
}
