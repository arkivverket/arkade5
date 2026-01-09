using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base;

public class AddmlXmlUnit(ArchiveXmlFile archiveXmlFile, ArchiveXmlSchema archiveXmlSchema)
    : ArchiveXmlUnit(archiveXmlFile, [archiveXmlSchema])
{
    public ArchiveXmlSchema Schema => Schemas[0];

    public void WriteFiles(ArkadeDirectory destinationDirectory)
    {
        using Stream addmlFileStream = File.AsStream();
        using Stream destinationAddmlFileStream = System.IO.File.Create(destinationDirectory.WithFile(File.Name).FullName);
        addmlFileStream.CopyTo(destinationAddmlFileStream);

        using Stream addmlSchemaStream = Schema.AsStream();
        using Stream destinationAddmlSchemaStream = System.IO.File.Create(destinationDirectory.WithFile(Schema.FileName).FullName);
        addmlSchemaStream.CopyTo(destinationAddmlSchemaStream);
    }
}
