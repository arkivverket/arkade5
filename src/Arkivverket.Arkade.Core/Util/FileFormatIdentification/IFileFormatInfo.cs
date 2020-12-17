namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public interface IFileFormatInfo
    {
        string FileName { get; }
        string FileExtension { get; }
        string Errors { get; }
        string Id { get; }
        string Format { get; }
        string Version { get; }
        string MimeType { get; }
    }
}
