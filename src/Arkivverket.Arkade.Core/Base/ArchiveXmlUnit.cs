using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveXmlUnit(ArchiveXmlFile file, List<ArchiveXmlSchema> schemas)
{
    public ArchiveXmlFile File { get; } = file;
    public List<ArchiveXmlSchema> Schemas { get; } = schemas;
}
