using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using FluentAssertions;
using static Arkivverket.Arkade.Core.Resources.OutputFileNames;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Tests.UnitTestUtilities;

public static class DiasTarArchiveUtility
{
    public static string[] GetContentFileList(string tarArchiveFilePath)
    {
        List<string> fileListFromTarArchive = GetFileList(tarArchiveFilePath);

        string packageUuidString = Path.GetFileNameWithoutExtension(tarArchiveFilePath);

        var contentDirectoryPath = $"{packageUuidString}/{DirectoryNameContent}/";

        IEnumerable<string> contentPaths = fileListFromTarArchive.Where(f => f.StartsWith(contentDirectoryPath));

        IEnumerable<string> filePathsUnderContent = contentPaths.Except([contentDirectoryPath]);

        string[] contentFileNames = filePathsUnderContent.Select(f => f[contentDirectoryPath.Length..]).ToArray();

        return contentFileNames;
    }

    public static List<string> GetFileList(string tarArchiveFilePath)
    {
        var tarArchiveFileList = new List<string>();

        using Stream tarArchiveStream = File.OpenRead(tarArchiveFilePath);

        using var tarArchiveReader = new TarReader(tarArchiveStream);

        while (tarArchiveReader.GetNextEntry() is { } entry)
            tarArchiveFileList.Add(entry.Name);

        return tarArchiveFileList;
    }

    public static string[] GetArkadeAppliedPackageFilesList(ArchiveType archiveType, PackageType packageType)
    {
        ArchiveFormat archiveFormat = (archiveType, packageType) switch
        {
            (ArchiveType.Noark5, PackageType.ArchivalInformationPackage) => ArchiveFormat.DiasAipN5,
            (ArchiveType.Noark5, PackageType.SubmissionInformationPackage) => ArchiveFormat.DiasSipN5,
            (ArchiveType.Siard, PackageType.ArchivalInformationPackage) => ArchiveFormat.DiasAipSiard,
            (ArchiveType.Siard, PackageType.SubmissionInformationPackage) => ArchiveFormat.DiasSipSiard,
            (_, PackageType.ArchivalInformationPackage) => ArchiveFormat.DiasAip, // Treated as SpecializedSystem
            (_, PackageType.SubmissionInformationPackage) => ArchiveFormat.DiasSip, // Treated as SpecializedSystem
            _ => throw new ArgumentOutOfRangeException()
        };

        // DiasProvider-supplied list of mandatory files: 
        var arkadeAppliedPackageFilesList = new List<string>(DiasProvider.ProvideForFormat(archiveFormat)
            .GetEntryPaths(recursive: true)
            .Where(d => !d.StartsWith(DirectoryNameContent + Path.DirectorySeparatorChar)) // Skipping content files
            .Select(p => p.Replace('\\', '/')));

        // EAD and EAC-CPF schemas are not included by Arkade during package creation
        arkadeAppliedPackageFilesList.Remove($"{DirectoryNameDescriptiveMetadata}/{EadXsdFileName}");
        arkadeAppliedPackageFilesList.Remove($"{DirectoryNameDescriptiveMetadata}/{EacCpfXsdFileName}");

        // Noark3 and Noark4 are not specifically supported by the DiasProvider.
        // For these archive types we are therefore calling its ProvideForFormat with DiasAip/DiasSip as archive format.
        // The DiasProvider is treating DiasAip/DiasSip as SpecializedSystem and includes addml files in adm. metadata.
        // Hence, these addml files need to be removed as they are not to be expected in a Noark3/Noark4-based package.
        if (archiveType is ArchiveType.Noark3 or ArchiveType.Noark4)
        {
            arkadeAppliedPackageFilesList.Remove($"{DirectoryNameAdministrativeMetadata}/{AddmlXmlFileName}"); 
            arkadeAppliedPackageFilesList.Remove($"{DirectoryNameAdministrativeMetadata}/{AddmlXsdFileName}");
        }

        // Not supplied by DiasProvider but always generated during package creation: 
        arkadeAppliedPackageFilesList.Add(LogXmlFileName);
        
        return arkadeAppliedPackageFilesList.ToArray();
    }

    public static List<string> GetPackageItemsExpectedInMetadata(List<string> packageFileList)
    {
        return packageFileList.Where(item => IsNotADirectoryItem(item) && IsNotTheMetadataFile(item)).ToList();

        bool IsNotADirectoryItem(string fileCandidate) =>
            !Path.EndsInDirectorySeparator(fileCandidate) &&
            Path.HasExtension(fileCandidate) && // Directory names containing dots "has extension" ...
            !packageFileList.Any(item => item.StartsWith(fileCandidate + '/') || item.StartsWith(fileCandidate + '\\'));
        // Trailing-slash-less dot-containing candidates not being the start of another list item path is undeterminable

        bool IsNotTheMetadataFile(string fileListItem) => !fileListItem.EndsWith(DiasMetsXmlFileName);
    }
}
