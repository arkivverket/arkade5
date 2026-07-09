using System;
using System.IO;
using System.Xml.Linq;

namespace Arkivverket.Arkade.Core.Util
{
    public static class LocalInfo
    {
        private static readonly FileInfo LocalInfoFile;

        static LocalInfo()
        {
            string appDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ArkadeConstants.DirectoryNameAppDataArkadeSubFolder
            );

            if (!Directory.Exists(appDataDirectory))
                Directory.CreateDirectory(appDataDirectory);

            MigrateFromLegacyAppDataFolder(appDataDirectory);

            LocalInfoFile = new FileInfo(Path.Combine(appDataDirectory, "local-info.xml"));

            if (!LocalInfoFile.Exists)
            {
                var locaInfoXmlDoc = new XDocument(
                    new XElement("localInfo",
                        new XElement("lastCheckForUpdate")));

                locaInfoXmlDoc.Save(LocalInfoFile.FullName);
            }
        }

        // Pre-rebranding (Arkivverket → Nasjonalarkivet) the per-user data lived in a folder named
        // after the old organisation. Carry our file over to the new folder and remove the old one if
        // it is left empty, so upgrades don't leave an orphaned folder behind. Best-effort: any failure
        // (or a non-empty legacy folder shared with another app) leaves the old folder untouched.
        private static void MigrateFromLegacyAppDataFolder(string currentAppDataDirectory)
        {
            try
            {
                string legacyDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    ArkadeConstants.LegacyDirectoryNameAppDataArkadeSubFolder
                );

                if (!Directory.Exists(legacyDirectory))
                    return;

                string legacyFile = Path.Combine(legacyDirectory, "local-info.xml");
                string currentFile = Path.Combine(currentAppDataDirectory, "local-info.xml");

                if (File.Exists(legacyFile) && !File.Exists(currentFile))
                    File.Move(legacyFile, currentFile);

                if (Directory.GetFileSystemEntries(legacyDirectory).Length == 0)
                    Directory.Delete(legacyDirectory);
            }
            catch
            {
                // Migration is best-effort; never let it break start-up.
            }
        }

        public static void SetTimeLastCheckForUpdate(DateTime timeLastCheckForUpdate)
        {
            XDocument localInfoFile = XDocument.Load(LocalInfoFile.FullName);

            localInfoFile.Element("localInfo").Element("lastCheckForUpdate").SetValue(
                timeLastCheckForUpdate.ToString("yyyy-MM-ddTHH:mm:ss")
            );

            localInfoFile.Save(LocalInfoFile.FullName);
        }

        public static DateTime? GetTimeLastCheckForUpdate()
        {
            try
            {
                XDocument localInfoFile = XDocument.Load(LocalInfoFile.FullName);

                string dateTimeString = localInfoFile.Element("localInfo")?.Element("lastCheckForUpdate")?.Value;

                return Convert.ToDateTime(dateTimeString);
            }
            catch
            {
                return null;
            }
        }
    }
}
