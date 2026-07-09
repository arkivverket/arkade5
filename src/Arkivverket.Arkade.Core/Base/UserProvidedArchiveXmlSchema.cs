using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public class UserProvidedXmlSchema(FileSystemInfo schemaFile) : ArchiveXmlSchema
{
    private readonly ArchiveXmlFile _archiveXmlFile = new(schemaFile);

    protected override string GetName()
    {
        return _archiveXmlFile.Name;
    }

    public override Stream AsStream()
    {
        return _archiveXmlFile.AsStream();
    }
}
