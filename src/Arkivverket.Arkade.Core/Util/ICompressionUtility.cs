using System.IO;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Util
{
    public interface ICompressionUtility
    {
        void ExtractFolderFromArchive(FileInfo file, DirectoryInfo targetDirectory, bool withoutDocumentFiles, Uuid uuid);

        void CompressFolderContentToArchiveFile(FileInfo targetFileName, DirectoryInfo sourceFileFolder);
    }
}
