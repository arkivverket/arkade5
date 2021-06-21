using System.IO;
using System.Reflection;
using Serilog;

namespace Arkivverket.Arkade.Core.Base
{
    public abstract class ArchiveXmlSchema
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public string FileName => GetFileName();
        
        public static UserProvidedXmlSchema Create(FileInfo xmlSchemaFile)
        {
            return new(xmlSchemaFile);
        }

        public static ArkadeBuiltInXmlSchema Create(string xmlSchemaFileName, string archiveTypeVersion)
        {
            Log.Warning(string.Format(Resources.Noark5Messages.InternalSchemaFileIsUsed, xmlSchemaFileName, archiveTypeVersion));
            
            return new ArkadeBuiltInXmlSchema(xmlSchemaFileName, archiveTypeVersion);
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
