using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedFileInfo : IEquatable<SiegfriedFileInfo>
    {
        public string FileName { get; }
        public string FileExtension { get; }
        public string Errors { get; }
        public string Id { get; }
        public string Format { get; }
        public string Version { get; }

        public SiegfriedFileInfo(string fileName, string errors, string id, string format, string version)
        {
            FileName = fileName;
            FileExtension = Path.GetExtension(fileName);
            Errors = errors;
            Id = id;
            Format = format;
            Version = version;
        }

        public bool Equals(SiegfriedFileInfo other)
        {
            return Id == other?.Id;
        }
    }
}
