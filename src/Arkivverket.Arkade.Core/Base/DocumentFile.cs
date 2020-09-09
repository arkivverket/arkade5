using System.IO;

namespace Arkivverket.Arkade.Core.Base
{
    public class DocumentFile
    {
        public FileInfo FileInfo { get; }
        public string CheckSum { get; set; }

        public DocumentFile(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }
    }
}
