using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArkadeBuiltInXmlSchema : ArchiveXmlSchema
    {
        private readonly string _xmlSchemaName;
        private readonly string _archiveTypeVersion;

        public ArkadeBuiltInXmlSchema(string xmlSchemaName, string archiveTypeVersion) // TODO: Accept construction without version
        {
            _xmlSchemaName = xmlSchemaName;
            _archiveTypeVersion = archiveTypeVersion;
        }

        protected override string GetFileName()
        {
            return _xmlSchemaName;
        }

        internal string GetArchiveTypeVersion()
        {
            return _archiveTypeVersion;
        }
        
        public override Stream AsStream() // TODO: Rewrite to handle schemas other than Noark5 (like addml.xsd ...)
        {
            string pathCompatibleVersionString = "v" + _archiveTypeVersion.Replace('.', '_');

            string xsdResourceName =
                $"{string.Format(ArkadeConstants.DirectoryPathNoark5XsdResources, pathCompatibleVersionString)}.{_xmlSchemaName}";

            return ResourceUtil.GetResourceAsStream(xsdResourceName);
        }
    }
}
