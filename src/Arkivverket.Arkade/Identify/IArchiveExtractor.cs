using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Identify
{
    public interface IArchiveExtractor
    {
        Archive Extract(string fileName);
    }
}