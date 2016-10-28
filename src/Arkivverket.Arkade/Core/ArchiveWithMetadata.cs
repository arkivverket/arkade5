namespace Arkivverket.Arkade.Core
{
    public class ArchiveWithMetadata
    {
        public string ArchiveFileName { get; }
        public string MetadataFileName { get; }

        public ArchiveWithMetadata(string archiveFileName, string metadataFileName)
        {
            ArchiveFileName = archiveFileName;
            MetadataFileName = metadataFileName;
        }
    }
}