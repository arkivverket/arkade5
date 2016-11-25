using System.IO;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class ArchiveDirectory
    {
        public DirectoryInfo Archive { get; }
        public FileInfo InfoXml { get; }

        private ArchiveDirectory(DirectoryInfo archive, FileInfo infoXml)
        {
            Archive = archive;
            InfoXml = infoXml;
        }

        public static ArchiveDirectory Read(string archiveDirectory)
        {
            return Read(new DirectoryInfo(archiveDirectory));
        }

        public static ArchiveDirectory Read(string archiveDirectory, string infoXml)
        {
            return Read(new DirectoryInfo(archiveDirectory), new FileInfo(infoXml));
        }

        public static ArchiveDirectory Read(DirectoryInfo archiveDirectory)
        {
            FileInfo infoXml = GetInfoXmlFile(archiveDirectory);
            return Read(archiveDirectory, infoXml);
        }


        private static ArchiveDirectory Read(DirectoryInfo archiveDirectory, FileInfo infoXml)
        {
            if (!archiveDirectory.Exists)
            {
                throw new ArkadeException("No such directory: " + archiveDirectory.FullName);
            }
            if (!infoXml.Exists)
            {
                throw new ArkadeException("No such file: " + infoXml.FullName);
            }

            return new ArchiveDirectory(archiveDirectory, infoXml);
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