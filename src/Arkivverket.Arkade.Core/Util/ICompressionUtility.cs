using System.IO;

namespace Arkivverket.Arkade.Core.Util
{
    public interface ICompressionUtility
    {
        void ExtractFolderFromArchive(FileInfo file, DirectoryInfo targetDirectory);

        void CompressFolderContentToArchiveFile(FileInfo targetFileName, DirectoryInfo sourceFileFolder);
    }
}
