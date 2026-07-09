using System.IO;

namespace Arkivverket.Arkade.Core.Util
{
    public interface ICompressionUtility
    {
        /// <summary>
        /// The name of the archive's single internal root directory, or null when the archive has
        /// no single root and its entry paths start at the package level.
        /// </summary>
        string GetRootDirectoryName(FileInfo file);

        /// <summary>
        /// Extracts the archive's contents into the target directory, relocating entries by
        /// stripping the given internal root directory name (obtain it with
        /// <see cref="GetRootDirectoryName"/>; pass null for an archive without a single root).
        /// </summary>
        void ExtractFolderFromArchive(FileInfo file, DirectoryInfo targetDirectory, bool withoutDocumentFiles,
            string archiveRootDirectoryName);

        void CompressFolderContentToArchiveFile(FileInfo targetFileName, DirectoryInfo sourceFileFolder);
    }
}
