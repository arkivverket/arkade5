using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArkadeBuiltInXmlSchema : ArchiveXmlSchema
    {
        private readonly string _xmlSchemaName;
        private readonly string _xsdResourceLocalPath;
        private readonly string _archiveTypeVersion;

        public ArkadeBuiltInXmlSchema(string xmlSchemaName, string xsdResourceLocalPath = null, string archiveTypeVersion = null)
        {
            _xmlSchemaName = xmlSchemaName;
            _xsdResourceLocalPath = xsdResourceLocalPath;
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

        public override Stream AsStream()
        {
            return ResourceUtil.GetResourceAsStream(
                $"{ArkadeConstants.DirectoryPathBuiltInXsdResources}.{_xsdResourceLocalPath}.{_xmlSchemaName}"
            );
        }

        // public override Stream AsStream() // TODO: Rewrite to handle schemas other than Noark5 (like addml.xsd ...)
        // {
        //     string pathCompatibleVersionString = "v" + _archiveTypeVersion.Replace('.', '_');
        //
        //     string xsdResourceName =
        //         $"{string.Format(ArkadeConstants.DirectoryPathNoark5XsdResources, pathCompatibleVersionString)}.{_xmlSchemaName}";
        //
        //     return ResourceUtil.GetResourceAsStream(xsdResourceName);
        // }
    }
}
