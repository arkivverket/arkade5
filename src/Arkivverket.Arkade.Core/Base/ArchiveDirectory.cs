using System.IO;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveDirectory
    {
        public DirectoryInfo Directory { get; }
        public ArchiveType ArchiveType { get; }

        private ArchiveDirectory(DirectoryInfo directory, ArchiveType archiveType)
        {
            Directory = directory;
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
    }
}