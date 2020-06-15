using System;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class FileFormat : IEquatable<FileFormat>
    {
        public string PuId { get; }
        public string Name { get; }
        public string Version { get; }
        public string MimeType { get; }

        public FileFormat(string puId, string name = "", string version = "", string mimeType = "")
        {
            PuId = puId;
            Name = name;
            Version = version;
            MimeType = mimeType;
        }

        public bool Equals(FileFormat other)
        {
            return PuId == other?.PuId;
        }
    }
}
