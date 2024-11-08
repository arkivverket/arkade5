using System.IO;
using Arkivverket.Arkade.Core.Languages;

namespace Arkivverket.Arkade.Core.Base;

public abstract class DiasPackage
{
    public Uuid Uuid { get; }
    public PackageType PackageType { get; }
    public Archive Archive { get; }
    public ArchiveMetadata ArchiveMetadata { get; }
    public FileInfo PhysicalPath { get; }

    protected DiasPackage(Uuid uuid, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata/*,
        FileInfo physicalPath*/)
    {
        Uuid = uuid;
        PackageType = packageType;
        Archive = archive;

        archiveMetadata.Id = $"UUID:{Uuid}";
        archiveMetadata.PackageType = packageType;
        ArchiveMetadata = archiveMetadata;
        //PhysicalPath = physicalPath;
    }

    public string GetInformationPackageFileName()
    {
        return Uuid + ".tar"; // NB! UUID-writeout (package creation)
    }

    public string GetSubmissionDescriptionFileName()
    {
        return Uuid + ".xml"; // NB! UUID-writeout (package creation)
    }
}

public class InputDiasPackage(Uuid uuid, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata/*, FileInfo physicalPath*/)
    : DiasPackage(uuid, packageType, archive, archiveMetadata/*, physicalPath*/)
{
}

public class OutputDiasPackage(PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata/*, FileInfo physicalPath*/, SupportedLanguage language, bool generateFileFormatInfo = false)
    : DiasPackage(Uuid.Random(), packageType, archive, archiveMetadata/*, physicalPath*/)
{
    public SupportedLanguage Language { get; } = language;
    public bool GenerateFileFormatInfo { get; } = generateFileFormatInfo;
}
