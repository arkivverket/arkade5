using System;
using System.IO;
using Serilog;

namespace Arkivverket.Arkade.Util
{
    public class ArkadeConstants
    {
        public const string NoarkihXmlFileName = "NOARKIH.XML";
        public const string AddmlXmlFileName = "addml.xml";
        public const string AddmlXsdFileName = "addml.xsd";
        public const string ArkivstrukturXmlFileName = "arkivstruktur.xml";
        public const string ArkivstrukturXsdFileName = "arkivstruktur.xsd";
        public const string InfoXmlFileName = "info.xml";
        public const string ArkadeXmlLogFileName = "arkade-log.xml";
        public const string EadXmlFileName = "ead.xml";
        public const string EacCpfXmlFileName = "eac-cpf.xml";
        public const string DiasPremisXmlFileName = "dias-premis.xml";
        public const string DiasMetsXmlFileName = "dias-mets.xml";
        public const string DiasMetsXsdFileName = "dias-mets.xsd";
        public const string ArkivuttrekkXmlFileName = "arkivuttrekk.xml";
        public const string PublicJournalXmlFileName = "offentligJournal.xml";
        public const string RunningJournalXmlFileName = "loependeJournal.xml";

        public const string AddmlXsdResource = "Arkivverket.Arkade.ExternalModels.xsd.addml.xsd";
        public const string ArkivstrukturXsdResource = "Arkivverket.Arkade.ExternalModels.xsd.arkivstruktur.xsd";
        public const string MetadatakatalogXsdResource = "Arkivverket.Arkade.ExternalModels.xsd.metadatakatalog.xsd";
        public const string DiasMetsXsdResource = "Arkivverket.Arkade.ExternalModels.xsd.DIAS_METS.xsd";

        public const string DirectoryNameRepositoryOperations = "repository_operations";
        public const string DirectoryNameContent = "content";

        private static readonly string UserHomeDirectoryString =
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        private static readonly string ArkadeDirectoryString = Path.Combine(UserHomeDirectoryString, "Arkade");
        private static readonly DirectoryInfo ArkadeDirectory = new DirectoryInfo(ArkadeDirectoryString);

        private static readonly string ArkadeWorkDirectoryString = Path.Combine(ArkadeDirectoryString, "work");
        private static readonly DirectoryInfo ArkadeWorkDirectory = new DirectoryInfo(ArkadeWorkDirectoryString);

        private static readonly string ArkadeLogDirectoryString = Path.Combine(ArkadeDirectoryString, "logs");
        private static readonly DirectoryInfo ArkadeLogDirectory = new DirectoryInfo(ArkadeLogDirectoryString);

        private static readonly string ArkadeIpDirectoryString = Path.Combine(ArkadeDirectoryString, "ip");
        private static readonly DirectoryInfo ArkadeIpDirectory = new DirectoryInfo(ArkadeIpDirectoryString);
        
        static ArkadeConstants()
        {
            if (!ArkadeDirectory.Exists)
            {
                ArkadeDirectory.Create();
                Log.Information("Arkade application directory created: " + ArkadeDirectory.FullName);
            }

            if (!ArkadeWorkDirectory.Exists)
            {
                ArkadeWorkDirectory.Create();
                Log.Information("Arkade temp directory created: " + ArkadeWorkDirectory.FullName);
            }

            if (!ArkadeIpDirectory.Exists)
            {
                ArkadeIpDirectory.Create();
                Log.Information("Arkade ip directory created: " + ArkadeIpDirectory.FullName);
            }

        }


        public static DirectoryInfo GetArkadeDirectory()
        {
            return ArkadeDirectory;
        }

        public static DirectoryInfo GetArkadeWorkDirectory()
        {
            return ArkadeWorkDirectory;
        }

        public static DirectoryInfo GetArkadeLogDirectory()
        {
            return ArkadeLogDirectory;
        }

        public static DirectoryInfo GetArkadeIpDirectory()
        {
            return ArkadeIpDirectory;
        }

        public static DirectoryInfo GetUniqueTempDirectory()
        {
            DirectoryInfo uniqueTempDir;

            // Be sure the directory does not already exist
            do
            {
                string dateString = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                uniqueTempDir = new DirectoryInfo(Path.Combine(ArkadeWorkDirectoryString, dateString));
            } while (!uniqueTempDir.Exists);

            uniqueTempDir.Create();

            return uniqueTempDir;
        }
    }
}