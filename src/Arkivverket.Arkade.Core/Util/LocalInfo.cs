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

            LocalInfoFile = new FileInfo(Path.Combine(appDataDirectory, "local-info.xml"));

            if (!LocalInfoFile.Exists)
            {
                var locaInfoXmlDoc = new XDocument(
                    new XElement("localInfo",
                        new XElement("lastCheckForUpdate")));

                locaInfoXmlDoc.Save(LocalInfoFile.FullName);
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
