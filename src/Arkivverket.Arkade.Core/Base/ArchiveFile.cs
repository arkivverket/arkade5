using System.IO;

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
    }
}