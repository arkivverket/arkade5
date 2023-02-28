namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public interface IFileSystemInfoSizeCalculator
    {
        public void CalculateSize(FileFormatScanMode scanMode, string target);
    }
}
