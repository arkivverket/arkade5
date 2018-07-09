using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveFile
    {
        public FileInfo File { get; }
        public ArchiveType ArchiveType { get; }

        private ArchiveFile(FileInfo file, ArchiveType archiveType)
        {
            File = file;
            ArchiveType = archiveType;
        }

        public static ArchiveFile Read(string archiveFile, ArchiveType archiveType)
        {
            return Read(new FileInfo(archiveFile), archiveType);
        }


        private static ArchiveFile Read(FileInfo archiveFile, ArchiveType archiveType)
        {
            if (!archiveFile.Exists)
            {
                throw new ArkadeException("No such file: " + archiveFile.FullName);
            }

            return new ArchiveFile(archiveFile, archiveType);
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