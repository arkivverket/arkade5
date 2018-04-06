using System.IO;

namespace Arkivverket.Arkade.Core
{
    public interface IArchiveContentReader
    {
        Stream GetContentAsStream(Archive archive);
        Stream GetStructureContentAsStream(Archive archive);
        Stream GetContentDescriptionXmlSchemaAsStream(Archive archive);
        Stream GetStructureDescriptionXmlSchemaAsStream(Archive archive);
        Stream GetMetadataCatalogXmlSchemaAsStream(Archive archive);
        Stream GetPublicJournalAsStream(Archive archive);
        Stream GetRunningJournalAsStream(Archive archive);
        Stream GetPublicJournalXmlSchemaAsStream(Archive archive);
        Stream GetRunningJournalXmlSchemaAsStream(Archive archive);
    }
}