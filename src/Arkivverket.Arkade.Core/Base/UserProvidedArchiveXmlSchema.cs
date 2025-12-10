using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public class UserProvidedXmlSchema(FileSystemInfo schemaFile) : ArchiveXmlSchema
{
    private readonly ArchiveXmlFile _archiveXmlFile = new(schemaFile);

    public string FullName => _archiveXmlFile.FullName;

    protected override string GetFileName()
    {
        return _archiveXmlFile.Name;
    }

    public override Stream AsStream()
    {
        return _archiveXmlFile.AsStream();
    }
}
