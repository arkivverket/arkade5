namespace Arkivverket.Arkade.Core.Base;

public class AddmlXmlUnit(ArchiveXmlFile archiveXmlFile, ArchiveXmlSchema archiveXmlSchema)
    : ArchiveXmlUnit(archiveXmlFile, [archiveXmlSchema])
{
    public ArchiveXmlSchema Schema => Schemas[0];
}
