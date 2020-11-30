using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Identify
{
    public interface IArchiveTypeIdentifier
    {
        ArchiveType? IdentifyTypeOfChosenArchiveDirectory(string archiveDirectoryName);
        ArchiveType? IdentifyTypeOfChosenArchiveFile(string archiveFileName);
    }
}
