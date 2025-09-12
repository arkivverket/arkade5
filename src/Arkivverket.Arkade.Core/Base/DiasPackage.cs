using System.IO;
using Arkivverket.Arkade.Core.ExternalModels.Noark5;

namespace Arkivverket.Arkade.Core.Base;

public abstract class DiasPackage
{
    public Uuid Id { get; }
    public PackageType PackageType { get; }
    public ArchiveMetadata ArchiveMetadata { get; }
    public DiasPackageWorkingDirectory WorkingDirectory { get; }

    protected DiasPackage(Uuid id, PackageType packageType, ArchiveMetadata archiveMetadata, DirectoryInfo archiveProcessingDirectory)
    {
        Id = id;
        PackageType = packageType;
        
        archiveMetadata.Id = $"UUID:{Id}"; // NB! UUID-writeout (package creation)
        archiveMetadata.PackageType = packageType;
        ArchiveMetadata = archiveMetadata;
        
        DirectoryInfo workingDirectoryRoot = archiveProcessingDirectory.CreateSubdirectory(id.GetValue());
        WorkingDirectory = new DiasPackageWorkingDirectory(workingDirectoryRoot);
    }
}

public class InputDiasPackage(Uuid id, PackageType packageType, ArchiveMetadata archiveMetadata, DirectoryInfo archiveProcessingDirectory)
    : DiasPackage(id, packageType, archiveMetadata, archiveProcessingDirectory);

public class OutputDiasPackage(PackageType packageType, ArchiveMetadata archiveMetadata, DirectoryInfo archiveProcessingDirectory)
    : DiasPackage(Uuid.Random(), packageType, archiveMetadata, archiveProcessingDirectory);
