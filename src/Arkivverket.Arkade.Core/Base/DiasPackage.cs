using System.IO;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Base;

public abstract class DiasPackage
{
    public Uuid Id { get; protected init; }
    public PackageType PackageType { get; protected init; }
    public ArchiveMetadata ArchiveMetadata { get; protected init; }
    public DiasPackageWorkingDirectory WorkingDirectory { get; private set; }

    protected void CreateWorkingDirectory(DirectoryInfo locationForWorkingDirectory)
    {
        DirectoryInfo workingDirectoryRoot = locationForWorkingDirectory.CreateSubdirectory(Id.GetValue());
        WorkingDirectory = new DiasPackageWorkingDirectory(workingDirectoryRoot);
    }

    public DirectoryInfo GetTestReportDirectory()
    {
        return WorkingDirectory.RepositoryOperations().WithSubDirectory(OutputFileNames.TestReportDirectory).DirectoryInfo();
    }
}

public class InputDiasPackage : DiasPackage
{
    public readonly FileInfo TarFile;

    public InputDiasPackage(FileInfo tarFile, ArchiveType archiveType, DirectoryInfo locationForWorkingDirectory, ICompressionUtility compressionUtility)
    {
        TarFile = tarFile;

        if (!Uuid.TryParse(Path.GetFileNameWithoutExtension(TarFile.Name), out Uuid id)) // NB! UUID-orig
            throw new ArkadeException("Could not extract an UUID from filename: " + tarFile.Name);

        Id = id;
        
        CreateWorkingDirectory(locationForWorkingDirectory);

        ExtractTarArchive(compressionUtility);

        ArchiveMetadata = MetadataLoader.Load(WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName).FullName);

        if (ArchiveMetadata.Id != $"UUID:{Id}") // NB! UUID-readin (package loading)
            Log.Warning($"Metadata ID ({ArchiveMetadata.Id}) does not match IP ID ({Id})");

        PackageType = ArchiveMetadata.PackageType;
    }

    private void ExtractTarArchive(ICompressionUtility compressionUtility)
    {
        //TarExtractionStartedEvent();
        compressionUtility.ExtractFolderFromArchive(TarFile, WorkingDirectory.Root().DirectoryInfo(),
            withoutDocumentFiles: false, archiveRootDirectoryName: Id.ToString());
        //TarExtractionFinishedEvent(workingDirectory);
    }
}

public class OutputDiasPackage : DiasPackage
{
    public OutputDiasPackage(PackageType packageType, ArchiveMetadata archiveMetadata, DirectoryInfo locationForWorkingDirectory)
    {
        Id = Uuid.Random(); // NB! UUID-orig

        CreateWorkingDirectory(locationForWorkingDirectory);

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
