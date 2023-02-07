namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public interface IFileCounter
    {
        public void CountFiles(FileFormatScanMode scanMode, string target);
    }
}
