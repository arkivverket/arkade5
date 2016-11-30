using System.IO;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class ArchiveDirectory
    {
        public DirectoryInfo Archive { get; }
        public ArchiveType ArchiveType { get; }

        private ArchiveDirectory(DirectoryInfo archive, ArchiveType archiveType)
        {
            Archive = archive;
            ArchiveType = archiveType;
        }

        public static ArchiveDirectory Read(string archiveDirectory, ArchiveType archiveType)
        {
            return Read(new DirectoryInfo(archiveDirectory), archiveType);
        }


        private static ArchiveDirectory Read(DirectoryInfo archiveDirectory, ArchiveType archiveType)
        {
            if (!archiveDirectory.Exists)
            {
                throw new ArkadeException("No such directory: " + archiveDirectory.FullName);
            }

            return new ArchiveDirectory(archiveDirectory, archiveType);
        }

        private static FileInfo GetInfoXmlFile(DirectoryInfo archiveDirectory)
        {
            if (archiveDirectory.Parent == null)
            {
                return null;
            }

            string infoXml = Path.Combine(archiveDirectory.Parent.FullName, ArkadeConstants.InfoXmlFileName);
            FileInfo fileInfo = new FileInfo(infoXml);
            return fileInfo.Exists ? fileInfo : null;
        }
    }
}