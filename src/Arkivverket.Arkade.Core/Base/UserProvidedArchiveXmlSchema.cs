using System.IO;

namespace Arkivverket.Arkade.Core.Base
{
    public class UserProvidedXmlSchema : ArchiveXmlSchema
    {
        private readonly ArchiveXmlFile _archiveXmlFile;

        public string FullName => _archiveXmlFile.FullName;
        public bool FileExists => _archiveXmlFile.Exists;

        public UserProvidedXmlSchema(FileSystemInfo schemaFile)
        {
            _archiveXmlFile = new ArchiveXmlFile(schemaFile);
        }

        protected override string GetFileName()
        {
            return _archiveXmlFile.Name;
        }

        public override Stream AsStream()
        {
            return _archiveXmlFile.AsStream();
        }
    }
}
