using System.IO;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class ArchiveFile
    {
        public FileInfo Archive { get; }
        public FileInfo InfoXml { get; }

        private ArchiveFile(FileInfo archive, FileInfo infoXml)
        {
            Archive = archive;
            InfoXml = infoXml;
        }

        public static ArchiveFile Read(string archiveFile)
        {
            return Read(new FileInfo(archiveFile));
        }

        public static ArchiveFile Read(string archiveFile, string infoXml)
        {
            return Read(new FileInfo(archiveFile), new FileInfo(infoXml));
        }

        public static ArchiveFile Read(FileInfo archiveFile)
        {
            FileInfo infoXml = GetInfoXmlFile(archiveFile);
            return Read(archiveFile, infoXml);
        }


        private static ArchiveFile Read(FileInfo archiveFile, FileInfo infoXml)
        {
            if (!archiveFile.Exists)
            {
                throw new ArkadeException("No such file: " + archiveFile.FullName);
            }
            if (!infoXml.Exists)
            {
                throw new ArkadeException("No such file: " + infoXml.FullName);
            }

            return new ArchiveFile(archiveFile, infoXml);
        }

        private static FileInfo GetInfoXmlFile(FileInfo tarFile)
        {
            if (tarFile.Directory == null)
            {
                return null;
            }

            string infoXml = Path.Combine(tarFile.Directory.FullName, ArkadeConstants.InfoXmlFileName);
            FileInfo fileInfo = new FileInfo(infoXml);
            return fileInfo.Exists ? fileInfo : null;
        }
        
    }
}