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
        addmlFileStream.CopyTo(System.IO.File.Create(destinationDirectory.WithFile(File.Name).FullName));

        using Stream addmlSchemaStream = Schema.AsStream();
        addmlSchemaStream.CopyTo(System.IO.File.Create(destinationDirectory.WithFile(Schema.FileName).FullName));
    }
}
