using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Logging
{
    public class StatusEventNewArchiveInformation
    {
        public string ArchiveType { get; set; }
        public string Uuid { get; set; }
        public DateTime StartTime { get; set; }
        public string ArchiveFileName { get; set; }

        public StatusEventNewArchiveInformation(string archiveType, string uuid, DateTime startTime, string archiveFileName)
        {
            ArchiveType = archiveType;
            Uuid = uuid;
            StartTime = startTime;
            ArchiveFileName = archiveFileName;
        }
    }
}
