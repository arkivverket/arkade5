using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveDetails
    {
        public string ArchiveCreators => GetArchiveCreators();
        public string ArchivalPeriod => GetArchivalPeriod();
        public string SystemName => GetSystemName();
        public string SystemType => GetSystemType();

        private readonly addml _addml;

        public ArchiveDetails(Archive archive)
        {
            _addml = SerializeUtil.DeserializeFromFile<addml>(archive.AddmlFile);
        }

        private string GetArchiveCreators()
        {
            try
            {
                IEnumerable<additionalElement> archiveCreators = GetAdditionalElements(
                    GetContextAdditionalElementsRoot(), "recordCreator"
                );

                return string.Join(", ", archiveCreators.Select(a => a.value));
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetArchivalPeriod()
        {
            try
            {
                additionalElement archivalPeriodElement = GetAdditionalElements(
                    GetContentAdditionalElementsRoot(), "archivalPeriod"
                ).FirstOrDefault();

                property startDate = archivalPeriodElement?.properties.FirstOrDefault(p => p.name.Equals("startDate"));
                property endDate = archivalPeriodElement?.properties.FirstOrDefault(p => p.name.Equals("endDate"));

                return $"{startDate?.value} - {endDate?.value}";
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetSystemName()
        {
            try
            {
                return GetAdditionalElements(GetContextAdditionalElementsRoot(), "systemName").FirstOrDefault()?.value;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetSystemType()
        {
            try
            {
                return GetAdditionalElements(GetContextAdditionalElementsRoot(), "systemType").FirstOrDefault()?.value;
            }
            catch
            {
                return string.Empty;
            }
        }

        private additionalElements GetContentAdditionalElementsRoot()
        {
            return _addml.dataset[0].reference.content.additionalElements;
        }

        private additionalElements GetContextAdditionalElementsRoot()
        {
            return _addml.dataset[0].reference.context.additionalElements;
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
