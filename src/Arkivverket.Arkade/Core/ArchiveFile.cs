using System.IO;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class ArchiveFile
    {
        public FileInfo Archive { get; }
        public ArchiveType ArchiveType { get; }

        private ArchiveFile(FileInfo archive, ArchiveType archiveType)
        {
            Archive = archive;
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