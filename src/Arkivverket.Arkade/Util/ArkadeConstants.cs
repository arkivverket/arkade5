using System;
using System.IO;
using Serilog;

namespace Arkivverket.Arkade.Util
{
    public class ArkadeConstants
    {
        public const string NoarkihXmlFileName = "NOARKIH.XML";
        public const string AddmlXmlFileName = "addml.xml";
        public const string InfoXmlFileName = "info.xml";

        private static readonly string UserHomeDirectoryString =
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        private static readonly string ArkadeDirectoryString = Path.Combine(UserHomeDirectoryString, "Arkade");
        private static readonly DirectoryInfo ArkadeDirectory = new DirectoryInfo(ArkadeDirectoryString);

        private static readonly string ArkadeWorkDirectoryString = Path.Combine(ArkadeDirectoryString, "work");
        private static readonly DirectoryInfo ArkadeWorkDirectory = new DirectoryInfo(ArkadeWorkDirectoryString);

        private static readonly string ArkadeLogDirectoryString = Path.Combine(ArkadeDirectoryString, "logs");
        private static readonly DirectoryInfo ArkadeLogDirectory = new DirectoryInfo(ArkadeLogDirectoryString);

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