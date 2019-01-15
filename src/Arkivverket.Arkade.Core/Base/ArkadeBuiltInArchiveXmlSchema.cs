using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArkadeBuiltInXmlSchema : ArchiveXmlSchema
    {
        private readonly string _xmlSchemaName;

        public ArkadeBuiltInXmlSchema(string xmlSchemaName)
        {
            _xmlSchemaName = xmlSchemaName;
        }

        protected override string GetFileName()
        {
            return _xmlSchemaName;
        }

        public override Stream AsStream()
        {
            string xsdResourceName = $"{ArkadeConstants.DirectoryPathBuiltInXsdResources}.{_xmlSchemaName}";

            return ResourceUtil.GetResourceAsStream(xsdResourceName);
        }
    }
}
