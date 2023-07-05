using System;

namespace Arkivverket.Arkade.Core.Base
{
    public class DocumentFile
    {
        public string FullName { get; init; }
        public string Extension { get; init; }
        public long Size { get; init; }
        public DateTime CreationTime { get; init; }
        public string CheckSum { get; set; }
    }
}
