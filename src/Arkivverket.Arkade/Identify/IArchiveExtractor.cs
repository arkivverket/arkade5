using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Identify
{
    public interface IArchiveExtractor
    {
        ArchiveExtraction Extract(string fileName);
    }
}