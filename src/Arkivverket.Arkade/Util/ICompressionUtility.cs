namespace Arkivverket.Arkade.Util
{
    public interface ICompressionUtility
    {
        int ExtractFolderFromArchive(string fileName, string targetFolderName);

        int CompressFolderContentToArchiveFile(string targetFileName, string sourceFileFolder);
    }
}
