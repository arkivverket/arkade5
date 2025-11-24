using System.IO;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Base;

public class ArkadeBuiltInXmlSchema : ArchiveXmlSchema
{
    public readonly Version SchemaVersion;
    private readonly string _xmlSchemaName;

    public ArkadeBuiltInXmlSchema(string xmlSchemaName, Version version = null)
    {
        // TODO: Use non-Noark5 specific warning message
        Log.Warning(string.Format(Resources.Noark5Messages.InternalSchemaFileIsUsed, xmlSchemaName, version?.Name));

        _xmlSchemaName = xmlSchemaName;
        SchemaVersion = version;
    }

    protected override string GetFileName()
    {
        return _xmlSchemaName;
    }

    public override Stream AsStream()
    {
        return ResourceUtil.GetResourceAsStream(SchemaVersion?.XsdResourceLocalPath != null
            ? $"{ArkadeConstants.DirectoryPathBuiltInXsdResources}.{SchemaVersion.XsdResourceLocalPath}.{_xmlSchemaName}"
            : $"{ArkadeConstants.DirectoryPathBuiltInXsdResources}.{_xmlSchemaName}");
    }

    public class Version(string name, string xsdResourceLocalPath = null)
    {
        public readonly string Name = name;
        internal readonly string XsdResourceLocalPath = xsdResourceLocalPath;
    }
}
