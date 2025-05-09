using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public abstract class DiasPackage
{
    public Uuid Id { get; }
    public PackageType PackageType { get; }
    public Archive Archive { get; }
    public ArchiveMetadata ArchiveMetadata { get; }
    public DiasPackageWorkingDirectory WorkingDirectory { get; }

    protected DiasPackage(Uuid id, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata, DirectoryInfo archiveProcessingDirectory)
    {
        Id = id;
        PackageType = packageType;
        Archive = archive;

        archiveMetadata.Id = $"UUID:{Id}"; // NB! UUID-writeout (package creation)
        archiveMetadata.PackageType = packageType;
        ArchiveMetadata = archiveMetadata;
        WorkingDirectory = new DiasPackageWorkingDirectory(archiveProcessingDirectory);
    }
}

public class InputDiasPackage(Uuid id, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata, DirectoryInfo archiveProcessingDirectory)
    : DiasPackage(id, packageType, archive, archiveMetadata, archiveProcessingDirectory);

public class OutputDiasPackage(PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata, DirectoryInfo archiveProcessingDirectory)
    : DiasPackage(Uuid.Random(), packageType, archive, archiveMetadata, archiveProcessingDirectory);
