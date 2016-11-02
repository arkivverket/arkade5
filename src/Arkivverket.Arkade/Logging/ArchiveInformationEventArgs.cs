using System;

namespace Arkivverket.Arkade.Logging
{
    public class ArchiveInformationEventArgs : EventArgs
    {
        public string ArchiveType { get; set; }
        public string Uuid { get; set; }
        public string ArchiveFileName { get; set; }

        public ArchiveInformationEventArgs(string archiveType, string uuid, string archiveFileName)
        {
            ArchiveType = archiveType;
            Uuid = uuid;
            ArchiveFileName = archiveFileName;
        }
    }
}