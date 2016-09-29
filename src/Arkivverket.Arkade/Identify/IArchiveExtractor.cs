using Arkivverket.Arkade.Core;
using System.IO;

namespace Arkivverket.Arkade.Identify
{
    public interface IArchiveExtractor
    {
        DirectoryInfo Extract(FileInfo fileName);
    }
}