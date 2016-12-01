using System.IO;

namespace Arkivverket.Arkade.Util
{
    public interface ICompressionUtility
    {
        void ExtractFolderFromArchive(FileInfo file, DirectoryInfo targetDirectory);

        void CompressFolderContentToArchiveFile(FileInfo targetFileName, DirectoryInfo sourceFileFolder);
    }
}
