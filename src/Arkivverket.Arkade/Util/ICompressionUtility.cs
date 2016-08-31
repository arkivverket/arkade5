namespace Arkivverket.Arkade.Util
{
    public interface ICompressionUtility
    {
        void ExtractFolderFromArchive(string fileName, string targetFolderName);

        void CompressFolderContentToArchiveFile(string targetFileName, string sourceFileFolder);
    }
}
