using System.IO;
using Arkivverket.Arkade.Core.Resources;
using Serilog;

namespace Arkivverket.Arkade.Core.Base;

public abstract class DiasPackage(DirectoryInfo locationForWorkingDirectory)
{
    public Uuid Id { get; protected init; }
    public ArchiveMetadata ArchiveMetadata { get; protected init; }
    public DiasPackageWorkingDirectory WorkingDirectory => _workingDirectory ?? CreateAndGetWorkingDirectory();
    
    private DiasPackageWorkingDirectory _workingDirectory;

    private DiasPackageWorkingDirectory CreateAndGetWorkingDirectory()
    {
        DirectoryInfo workingDirectoryRoot = locationForWorkingDirectory.CreateSubdirectory(Id.GetValue());
        _workingDirectory = new DiasPackageWorkingDirectory(workingDirectoryRoot);

        return WorkingDirectory;
    }

    public DirectoryInfo GetTestReportDirectory()
    {
        return WorkingDirectory.RepositoryOperations().WithSubDirectory(OutputFileNames.TestReportDirectory).DirectoryInfo();
    }
}

public sealed class InputDiasPackage : DiasPackage
{
    public readonly FileInfo TarFile;

    public InputDiasPackage(FileInfo tarFile, DirectoryInfo locationForWorkingDirectory) : base(locationForWorkingDirectory)
    {
        if (!Uuid.TryParse(Path.GetFileNameWithoutExtension(tarFile.Name), out Uuid id)) // NB! UUID-orig
            throw new ArkadeException("Could not extract an UUID from filename: " + tarFile.Name);

        Id = id;
        TarFile = tarFile;
    }
}

public sealed class OutputDiasPackage : DiasPackage
{
    public readonly PackageType PackageType;
    
    public OutputDiasPackage(PackageType packageType, ArchiveMetadata archiveMetadata, DirectoryInfo locationForWorkingDirectory) : base(locationForWorkingDirectory)
    {
        Id = Uuid.Random(); // NB! UUID-orig

        PackageType = packageType;

        ArchiveMetadata = archiveMetadata;

        ArchiveMetadata.Id = $"UUID:{Id}"; // NB! UUID-writeout (package creation)
        ArchiveMetadata.PackageType = PackageType;

        if (archiveMetadata.Id != ArchiveMetadata.Id)
            Log.Warning($"Metadata ID was set to IP ID ({Id})");

        if (archiveMetadata.PackageType != ArchiveMetadata.PackageType)
            Log.Warning($"Metadata package type was set to IP package type ({PackageType})");
    }
}
