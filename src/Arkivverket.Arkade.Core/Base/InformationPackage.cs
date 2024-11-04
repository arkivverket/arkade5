using Arkivverket.Arkade.Core.Languages;
namespace Arkivverket.Arkade.Core.Base;

public abstract class InformationPackage(Uuid uuid, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata)
{
    public Uuid Uuid { get; } = uuid;
    public PackageType PackageType { get; } = packageType;
    public Archive Archive { get; } = archive;
    public ArchiveMetadata ArchiveMetadata { get; } = archiveMetadata;


    public string GetInformationPackageFileName()
    {
        return Uuid + ".tar"; // NB! UUID-writeout (package creation)
    }

    public string GetSubmissionDescriptionFileName()
    {
        return Uuid + ".xml"; // NB! UUID-writeout (package creation)
    }
}

public class InputInformationPackage(Uuid uuid, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata) : InformationPackage(uuid, packageType, archive, archiveMetadata)
{
}

public class OutputInformationPackage(PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata, SupportedLanguage language, bool generateFileFormatInfo) : InformationPackage(Uuid.Random(), packageType, archive, archiveMetadata)
{
    public SupportedLanguage Language { get; } = language;
    public bool GenerateFileFormatInfo { get; set; } = generateFileFormatInfo;
}
