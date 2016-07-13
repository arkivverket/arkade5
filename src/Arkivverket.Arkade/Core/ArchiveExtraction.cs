namespace Arkivverket.Arkade.Core
{
    public class ArchiveExtraction
    {
        public ArchiveExtraction(string uuid, string workingDirectory)
        {
            Uuid = uuid;
            WorkingDirectory = workingDirectory;
        }

        public string Uuid { get; private set; }
        public string WorkingDirectory { get; private set; }
        public ArchiveType ArchiveType { get; set; }
    }

    public enum ArchiveType
    {
        Noark3,
        Noark4,
        Noark5,
        Fagsystem,
    }

}