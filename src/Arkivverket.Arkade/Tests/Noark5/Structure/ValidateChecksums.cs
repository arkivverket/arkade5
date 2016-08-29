using System.Linq;
using System.Xml.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class ValidateChecksums : BaseTest
    {
        protected override void Test(ArchiveExtraction archive)
        {
            addml structure = SerializeUtil.DeserializeFromString<addml>(archive.GetStructureDescriptionFileName());

            foreach (dataObject entry in structure.dataset[0].dataObjects.dataObject)
            {
                foreach (dataObject currentObject in entry.dataObjects.dataObject)
                {
                    foreach (property fileProperty in currentObject.properties.Where(s => s.name == "file"))
                    {
                        property fileNameProperty = fileProperty.properties.FirstOrDefault(p => p.name == "name");
                        property checksumProperty = fileProperty.properties.FirstOrDefault(p => p.name == "checksum");

                    }
                }
            }

            //archive.GetStructureDescriptionFileName()
        }
    }
}
