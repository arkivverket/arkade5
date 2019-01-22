using System.IO;

namespace Arkivverket.Arkade.Core.Base
{
    public abstract class ArchiveXmlSchema
    {
        public string FileName => GetFileName();
        
        public static ArchiveXmlSchema Create(FileInfo xmlSchemaFile)
        {
            if (xmlSchemaFile.Exists)
                return new UserProvidedXmlSchema(xmlSchemaFile);

            return new ArkadeBuiltInXmlSchema(xmlSchemaFile.Name);
        }

        public bool IsUserProvided()
        {
            return GetType() == typeof(UserProvidedXmlSchema);
        }

        public bool IsArkadeBuiltIn()
        {
            return GetType() == typeof(ArkadeBuiltInXmlSchema);
        }

        protected abstract string GetFileName();
        public abstract Stream AsStream();
    }
}
