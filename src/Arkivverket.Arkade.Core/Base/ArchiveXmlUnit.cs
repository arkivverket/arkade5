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

        public ArchiveXmlUnit(ArchiveXmlFile file, ArchiveXmlSchema schema)
            : this(file, new List<ArchiveXmlSchema> {schema})
        {
        }

        public bool AllFilesExists()
        {
            return !GetMissingFiles().Any();
        }

        public IEnumerable<string> GetMissingFiles()
        {
            var missingFiles = new List<string>();

            if (!File.Exists)
                missingFiles.Add(File.Name);

            missingFiles.AddRange(
                from schema in
                    from schema in Schemas
                    where schema.IsUserProvided()
                    select (UserProvidedXmlSchema) schema
                where !schema.FileExists
                select schema.FullName
            );

            return missingFiles;
        }
    }
}
