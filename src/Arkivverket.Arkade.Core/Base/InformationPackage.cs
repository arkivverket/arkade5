using Arkivverket.Arkade.Core.Languages;
namespace Arkivverket.Arkade.Core.Base;

public abstract class InformationPackage
{
    public Uuid Uuid { get; }
    public PackageType PackageType { get; }
    public Archive Archive { get; }
    public ArchiveMetadata ArchiveMetadata { get; }

    protected InformationPackage(Uuid uuid, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata)
    {
        Uuid = uuid;
        PackageType = packageType;
        Archive = archive;
        
        archiveMetadata.Id = $"UUID:{Uuid}";
        ArchiveMetadata = archiveMetadata;
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

public class InputInformationPackage(Uuid uuid, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata) : InformationPackage(uuid, packageType, archive, archiveMetadata)
{
}

public class OutputInformationPackage(PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata, SupportedLanguage language, bool generateFileFormatInfo = false) : InformationPackage(Uuid.Random(), packageType, archive, archiveMetadata)
{
    public SupportedLanguage Language { get; } = language;
    public bool GenerateFileFormatInfo { get; } = generateFileFormatInfo;
}
