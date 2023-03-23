using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Base
{
    public class DocumentFile
    {
        public FileInfo FileInfo { get; }
        public string Extension { get; init; }
        public long Size { get; init; }
        public DateTime CreationTime { get; init; } 
        public string CheckSum { get; set; }

        public DocumentFile(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }
    }
}
