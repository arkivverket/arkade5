using System.Collections.Generic;
using System.Linq;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveXmlUnit
    {
        public ArchiveXmlFile File { get; }
        public List<ArchiveXmlSchema> Schemas { get; }

        public ArchiveXmlUnit(ArchiveXmlFile file, List<ArchiveXmlSchema> schemas)
        {
            File = file;
            Schemas = schemas;
        }

        protected ArchiveXmlUnit(ArchiveXmlFile file, ArchiveXmlSchema schema)
            : this(file, new List<ArchiveXmlSchema> {schema})
        {
        }
    }
}
