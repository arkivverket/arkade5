using System.IO;
using Arkivverket.Arkade.Core.Languages;

namespace Arkivverket.Arkade.Core.Base;

public abstract class DiasPackage
{
    public Uuid Id { get; }
    public PackageType PackageType { get; }
    public Archive Archive { get; }
    public ArchiveMetadata ArchiveMetadata { get; }
    public FileInfo PhysicalPath { get; }

    protected DiasPackage(Uuid id, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata/*,
        FileInfo physicalPath*/)
    {
        Id = id;
        PackageType = packageType;
        Archive = archive;

        archiveMetadata.Id = $"UUID:{Id}"; // NB! UUID-writeout (package creation)
        archiveMetadata.PackageType = packageType;
        ArchiveMetadata = archiveMetadata;
        //PhysicalPath = physicalPath;
    }

    public string GetInformationPackageFileName()
    {
        return Id + ".tar"; // NB! UUID-writeout (package creation)
    }

    public string GetSubmissionDescriptionFileName()
    {
        return Id + ".xml"; // NB! UUID-writeout (package creation)
    }
}

public class InputDiasPackage(Uuid id, PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata/*, FileInfo physicalPath*/)
    : DiasPackage(id, packageType, archive, archiveMetadata/*, physicalPath*/)
{
}

public class OutputDiasPackage(PackageType packageType, Archive archive, ArchiveMetadata archiveMetadata/*, FileInfo physicalPath*/, SupportedLanguage language, bool generateFileFormatInfo = false)
    : DiasPackage(Uuid.Random(), packageType, archive, archiveMetadata/*, physicalPath*/)
{
    public SupportedLanguage Language { get; } = language;
    public bool GenerateFileFormatInfo { get; } = generateFileFormatInfo;
}
