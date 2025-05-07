namespace Arkivverket.Arkade.Core.Base;

public abstract class DiasPackage
{
    public Uuid Id { get; }
    public PackageType PackageType { get; }
    public Archive Archive { get; }
    public ArchiveMetadata ArchiveMetadata { get; }
    public WorkingDirectory WorkingDirectory { get; set; }

    protected DiasPackage(Uuid id, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata)
    {
        Id = id;
        PackageType = packageType;
        Archive = archive;

        archiveMetadata.Id = $"UUID:{Id}"; // NB! UUID-writeout (package creation)
        archiveMetadata.PackageType = packageType;
        ArchiveMetadata = archiveMetadata;
    }
}

public class InputDiasPackage(Uuid id, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata)
    : DiasPackage(id, packageType, archive, archiveMetadata);

public class OutputDiasPackage(PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata)
    : DiasPackage(Uuid.Random(), packageType, archive, archiveMetadata);
