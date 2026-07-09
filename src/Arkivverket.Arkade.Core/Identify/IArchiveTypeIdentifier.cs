using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;

namespace Arkivverket.Arkade.Core.Identify
{
    public interface IArchiveTypeIdentifier
    {
        ArchiveType? IdentifyTypeOfChosenArchiveDirectory(string archiveDirectoryName);
        ArchiveType? IdentifyTypeOfChosenArchiveFile(string archiveFileName);
    }
}
