namespace Arkivverket.Arkade.Core.Base
{
    public class AddmlXmlUnit : ArchiveXmlUnit
    {
        public ArchiveXmlSchema Schema
        {
            get => Schemas[0];
            set => Schemas[0] = value;
        }

        public AddmlXmlUnit(ArchiveXmlFile archiveXmlFile, ArchiveXmlSchema archiveXmlSchema)
        : base(archiveXmlFile, archiveXmlSchema)
        {
        }
    }
}
