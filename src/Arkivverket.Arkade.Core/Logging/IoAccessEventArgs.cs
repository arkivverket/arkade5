using System.IO;

namespace Arkivverket.Arkade.Core.Logging
{
    public class IoAccessEventArgs
    {
        public DirectoryInfo Location { get; }
        public IoAccessType IoAccessType { get; }

        public IoAccessEventArgs(DirectoryInfo location, IoAccessType ioAccessType)
        {
            Location = location;
            IoAccessType = ioAccessType;
        }
    }

    public enum IoAccessType
    {
        Read,
        Write
    }
}
