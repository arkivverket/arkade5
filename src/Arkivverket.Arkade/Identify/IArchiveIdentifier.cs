using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Identify
{
    public interface IArchiveIdentifier
    {
        ArchiveType Identify(string metadataFileName);
    }
}
