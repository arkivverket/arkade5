using System.IO;

namespace Arkivverket.Arkade.Core.Util
{
    public interface ICompressionUtility
    {
        void ExtractFolderFromArchive(FileInfo file, DirectoryInfo targetDirectory, bool withoutDocumentFiles, 
            string archiveRootDirectoryName);

        void CompressFolderContentToArchiveFile(FileInfo targetFileName, DirectoryInfo sourceFileFolder);
    }
}
