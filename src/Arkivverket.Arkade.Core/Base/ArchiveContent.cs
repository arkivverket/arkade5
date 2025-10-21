using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveContent(FileSystemInfo source)
{
    public FileSystemInfo Source { get; } = source;
}