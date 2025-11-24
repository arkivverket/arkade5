using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArkadeBuiltInXmlSchema : ArchiveXmlSchema
    {
        public readonly Version SchemaVersion;
        private readonly string _xmlSchemaName;

        public ArkadeBuiltInXmlSchema(string xmlSchemaName, ArkadeBuiltInXmlSchema.Version version = null)
        {
            _xmlSchemaName = xmlSchemaName;
            SchemaVersion = version;
        }

        protected override string GetFileName()
        {
            return _xmlSchemaName;
        }

        public override Stream AsStream()
        {
            return ResourceUtil.GetResourceAsStream(SchemaVersion != null
                ? $"{ArkadeConstants.DirectoryPathBuiltInXsdResources}.{SchemaVersion.XsdResourceLocalPath}.{_xmlSchemaName}"
                : $"{ArkadeConstants.DirectoryPathBuiltInXsdResources}.{_xmlSchemaName}");
        }

        public class Version(string name, string xsdResourceLocalPath)
        {
            public readonly string Name = name;
            internal readonly string XsdResourceLocalPath = xsdResourceLocalPath;
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
