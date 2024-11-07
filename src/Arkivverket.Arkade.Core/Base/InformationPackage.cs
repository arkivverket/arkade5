using System.IO;
using Arkivverket.Arkade.Core.Languages;

namespace Arkivverket.Arkade.Core.Base;

public abstract class InformationPackage // TODO: Rename to DiasPackage?
{
    public Uuid Uuid { get; }
    public PackageType PackageType { get; }
    public Archive Archive { get; }
    public ArchiveMetadata ArchiveMetadata { get; }
    public FileInfo PhysicalPath { get; }

    protected InformationPackage(Uuid uuid, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata/*,
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

public class InputInformationPackage(Uuid uuid, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata/*, FileInfo physicalPath*/)
    : InformationPackage(uuid, packageType, archive, archiveMetadata/*, physicalPath*/) // TODO: Rename to InputDiasPackage?
{
}

public class OutputInformationPackage(PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata/*, FileInfo physicalPath*/, SupportedLanguage language, bool generateFileFormatInfo = false)
    : InformationPackage(Uuid.Random(), packageType, archive, archiveMetadata/*, physicalPath*/) // TODO: Rename to OutputDiasPackage?
{
    public SupportedLanguage Language { get; } = language;
    public bool GenerateFileFormatInfo { get; } = generateFileFormatInfo;
}
