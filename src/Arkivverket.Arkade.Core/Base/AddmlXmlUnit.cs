namespace Arkivverket.Arkade.Core.Base;

public class AddmlXmlUnit(ArchiveXmlFile archiveXmlFile, ArchiveXmlSchema archiveXmlSchema)
    : ArchiveXmlUnit(archiveXmlFile, archiveXmlSchema)
{
    public ArchiveXmlSchema Schema
    {
        get => Schemas[0];
        set => Schemas[0] = value;
    }

    internal bool HasNoDefinedSchema()
    {
        return Schema == null;
    }
}
