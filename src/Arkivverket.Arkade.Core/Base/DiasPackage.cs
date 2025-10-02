using System;
using System.Formats.Tar;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.Noark5;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base;

public abstract class DiasPackage
{
    public Uuid Id { get; }
    public ArchiveMetadata ArchiveMetadata { get; }
    public DiasPackageWorkingDirectory WorkingDirectory { get; }
    
    protected DiasPackage(Uuid id, ArchiveMetadata archiveMetadata, DirectoryInfo archiveProcessingDirectory)
    {
        Id = id;

        ArchiveMetadata = archiveMetadata;
        
        DirectoryInfo workingDirectoryRoot = archiveProcessingDirectory.CreateSubdirectory(id.GetValue());
        WorkingDirectory = new DiasPackageWorkingDirectory(workingDirectoryRoot);
    }

    public DirectoryInfo GetTestReportDirectory()
    {
        return WorkingDirectory.RepositoryOperations().WithSubDirectory(OutputFileNames.TestReportDirectory).DirectoryInfo();
    }
}

public class InputDiasPackage(FileInfo inputDiasPackageTarFile, DirectoryInfo archiveProcessingDirectory) : DiasPackage(GetUuid(inputDiasPackageTarFile), null, null, archiveProcessingDirectory)
{
    public readonly FileInfo TarFile = inputDiasPackageTarFile;

    private static Uuid GetUuid(FileInfo inputDiasPackageTarFile)
    {
        if (!Uuid.TryParse(Path.GetFileNameWithoutExtension(inputDiasPackageTarFile.Name), out Uuid uuid)) // NB! UUID-orig
            throw new ArkadeException("Could not extract an UUID from filename: " + inputDiasPackageTarFile.Name);

        return uuid;
    }
}

public class OutputDiasPackage(PackageType packageType, ArchiveMetadata archiveMetadata, DirectoryInfo archiveProcessingDirectory) : DiasPackage(_uuid, AlignWithIp(archiveMetadata), archiveProcessingDirectory)
{
    private Uuid _uuid = Uuid.Random();

    private static ArchiveMetadata AlignWithIp(ArchiveMetadata archiveMetadata)
    {
        archiveMetadata.Id = $"UUID:{_uuid.ToString()}"; // NB! UUID-writeout (package creation)
        archiveMetadata.PackageType = packageType;

        return archiveMetadata;
    }
}
