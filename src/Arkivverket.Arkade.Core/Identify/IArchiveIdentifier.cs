using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Identify
{
    public interface IArchiveIdentifier
    {
        ArchiveType Identify(string metadataFileName);
    }
}
