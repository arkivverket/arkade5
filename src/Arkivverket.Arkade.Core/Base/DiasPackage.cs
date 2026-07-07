using System;
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
    public DiasPackageWorkingDirectory WorkingDirectory { get; protected init; }

    public DirectoryInfo GetContentDirectory()
    {
        return WorkingDirectory.ContentWorkDirectory().DirectoryInfo();
    }
    
    public DirectoryInfo GetTestReportDirectory()
    {
        return WorkingDirectory.RepositoryOperations().WithSubDirectory(OutputFileNames.TestReportDirectory).DirectoryInfo();
    }
}

public class InputDiasPackage : DiasPackage
{
    public readonly FileInfo TarFile;

    /// <summary>
    /// The name of the source tar's single internal root directory (null when the tar has no
    /// single root). For a well-formed DIAS package it equals the package UUID, but it is read
    /// from the tar's actual structure and carries no identity authority.
    /// </summary>
    public string TarRootDirectoryName { get; }

    public InputDiasPackage(DiasPackageWorkingDirectory workingDirectory, FileInfo tarFile, string tarRootDirectoryName)
    {
        WorkingDirectory = workingDirectory;
        TarFile = tarFile;
        TarRootDirectoryName = tarRootDirectoryName;

        // The package's METS (OBJID) is the authoritative source of its identity — file and
        // directory names carry none. A package whose METS is missing or lacks a valid UUID is
        // still loaded, but with a null Id (its test report is then identified by time of testing).
        FileInfo metsFile = WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName);

        if (metsFile.Exists)
        {
            try
            {
                ArchiveMetadata = MetadataLoader.Load(metsFile.FullName);
            }
            catch (Exception e)
            {
                Log.Warning($"Could not read package metadata from {ArkadeConstants.DiasMetsXmlFileName}: {e.Message}");
            }
        }
        else
            Log.Warning($"The package contains no {ArkadeConstants.DiasMetsXmlFileName}");

        if (Uuid.TryParseFromMetsObjid(ArchiveMetadata?.Id, out Uuid id) || // NB! UUID-readin (package loading)
            Uuid.TryParseFromMetsObjid(ReadMetsRootObjid(metsFile), out id))
            Id = id;
        else
            Log.Warning("No valid package identity (UUID) was found in the package's METS OBJID");

        if (Id != null && tarFile != null &&
            (!Uuid.TryParse(Path.GetFileNameWithoutExtension(tarFile.Name), out Uuid fileNameUuid) ||
             !fileNameUuid.Equals(Id)))
            Log.Warning($"Package file {tarFile.Name} is not named by the package identity ({Id})");

        PackageType = ArchiveMetadata?.PackageType ?? default;
    }

    // OBJID sits as an attribute on the METS root element in every METS dialect, while the full
    // metadata model reads the current DIAS METS namespace only — packages produced by older tools
    // (e.g. LOC-namespaced eARD-profile METS) would otherwise lose their declared identity.
    private static string ReadMetsRootObjid(FileInfo metsFile)
    {
        if (!metsFile.Exists)
            return null;

        try
        {
            using var xmlReader = System.Xml.XmlReader.Create(metsFile.FullName);
            xmlReader.MoveToContent();
            return xmlReader.GetAttribute("OBJID");
        }
        catch (Exception e)
        {
            Log.Warning($"Could not read OBJID from {ArkadeConstants.DiasMetsXmlFileName}: {e.Message}");
            return null;
        }
    }
}

public class OutputDiasPackage : DiasPackage
{
    public OutputDiasPackage(PackageType packageType, ArchiveMetadata archiveMetadata, DirectoryInfo locationForWorkingDirectory)
    {
        Id = Uuid.Random(); // NB! UUID-orig

        DirectoryInfo workingDirectoryRoot = locationForWorkingDirectory.CreateSubdirectory(Id.GetValue());
        WorkingDirectory = new DiasPackageWorkingDirectory(workingDirectoryRoot);
        WorkingDirectory.CreateDirectories(packageType);
        
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
