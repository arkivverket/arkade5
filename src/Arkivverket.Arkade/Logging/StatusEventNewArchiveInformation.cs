namespace Arkivverket.Arkade.Logging
{
    public class StatusEventNewArchiveInformation
    {
        public string ArchiveType { get; set; }
        public string Uuid { get; set; }
        public string ArchiveFileName { get; set; }

        public StatusEventNewArchiveInformation(string archiveType, string uuid, string archiveFileName)
        {
            ArchiveType = archiveType;
            Uuid = uuid;
            ArchiveFileName = archiveFileName;
        }
    }
}