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

        public static ArkadeBuiltInXmlSchema Create(string xmlSchemaFileName)
        {
            Log.Warning(string.Format(Resources.Noark5Messages.InternalSchemaFileIsUsed, xmlSchemaFileName));
            
            return new ArkadeBuiltInXmlSchema(xmlSchemaFileName);
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
