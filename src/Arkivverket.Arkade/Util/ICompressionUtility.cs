namespace Arkivverket.Arkade.Util
{
    public interface ICompressionUtility
    {
        //TODO: FileInfo and DirectoryInfo should be used here
        void ExtractFolderFromArchive(string fileName, string targetFolderName);

        void CompressFolderContentToArchiveFile(string targetFileName, string sourceFileFolder);
    }
}
