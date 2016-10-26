using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Arkivverket.Arkade.Core
{
    public class ArkadeConstants
    {
        private static readonly string UserHomeDirectoryString = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        private static readonly string ArkadeDirectoryString = Path.Combine(UserHomeDirectoryString, "Arkade");
        private static readonly DirectoryInfo ArkadeDirectory = new DirectoryInfo(ArkadeDirectoryString);

        private static readonly string ArkadeTempDirectoryString = Path.Combine(ArkadeDirectoryString, "temp");
        private static readonly DirectoryInfo ArkadeTempDirectory = new DirectoryInfo(ArkadeTempDirectoryString);

        private static readonly string ArkadeLogDirectoryString = Path.Combine(ArkadeDirectoryString, "logs");
        private static readonly DirectoryInfo ArkadeLogDirectory = new DirectoryInfo(ArkadeLogDirectoryString);
        
        static ArkadeConstants()
        {
            if (!ArkadeDirectory.Exists)
            {
                ArkadeDirectory.Create();
                Log.Information("Arkade application directory created: " + ArkadeDirectory.FullName);
            }

            if (!ArkadeTempDirectory.Exists)
            {
                ArkadeTempDirectory.Create();
                Log.Information("Arkade temp directory created: " + ArkadeTempDirectory.FullName);
            }


        }


        public static DirectoryInfo GetArkadeDirectory()
        {
            return ArkadeDirectory;
        }

        public static DirectoryInfo GetArkadeTempDirectory()
        {
            return ArkadeTempDirectory;
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
                string dateString = DateTime.Now.ToString("yyyyMMddhhmmssffff");
                uniqueTempDir = new DirectoryInfo(Path.Combine(ArkadeTempDirectoryString, dateString));
            } while (!uniqueTempDir.Exists);

            uniqueTempDir.Create();

            return uniqueTempDir;
        }



    }
}
