using System;
using System.IO;
using System.Text;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedFileInfo : IFileFormatInfo, IEquatable<SiegfriedFileInfo>
    {
        public string FileName { get; }
        public string ByteSize { get; }
        public string FileExtension { get; }
        public string Errors { get; }
        public string Id { get; }
        public string Format { get; }
        public string Version { get; }
        public string MimeType { get; }

        public SiegfriedFileInfo(string fileName, string byteSize, string errors, string id, string format, string version, string mimeType)
        {
            FileName = fileName;
            ByteSize = byteSize;
            FileExtension = Path.GetExtension(fileName);
            Errors = errors;
            Id = id;
            Format = format;
            Version = version;
            MimeType = mimeType;
        }

        public bool Equals(SiegfriedFileInfo other)
        {
            return Id == other?.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is SiegfriedFileInfo info && Equals(info);
        }

        public override int GetHashCode()
        {
            var sb = new StringBuilder();
            sb.Append(FileName);
            sb.Append(ByteSize);
            sb.Append(FileExtension);
            sb.Append(Errors);
            sb.Append(Id);
            sb.Append(Version);
            sb.Append(MimeType);
            return sb.ToString().GetHashCode();
        }
    }
}
