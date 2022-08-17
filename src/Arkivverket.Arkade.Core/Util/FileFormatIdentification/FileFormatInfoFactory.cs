namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public static class FileFormatInfoFactory
    {
        public static IFileFormatInfo Create(string fileName, string errors, string id, string format, string version,
            string mimeType)
        {
            return new SiegfriedFileInfo(fileName, errors, id, format, version, mimeType);
        }
    }
}
