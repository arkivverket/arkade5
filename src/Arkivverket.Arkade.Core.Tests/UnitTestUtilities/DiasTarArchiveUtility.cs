using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Archives;
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

    public static string[] GetArkadeAppliedAipFilesList(ArchiveType archiveType)
    {
        List<string> fileList =
        [
            DiasMetsXmlFileName,
            DiasMetsXsdFileName,
            $"{DirectoryNameAdministrativeMetadata}/",
            $"{DirectoryNameDescriptiveMetadata}/",
            $"{DirectoryNameContent}/",
            $"{DirectoryNameAdministrativeMetadata}/{DiasPremisXmlFileName}",
            $"{DirectoryNameAdministrativeMetadata}/{DiasPremisXsdFileName}",
            LogXmlFileName
        ];

        switch (archiveType)
        {
            case ArchiveType.Noark5:
                fileList.Add($"{DirectoryNameAdministrativeMetadata}/{ArkivuttrekkXmlFileName}");
                fileList.Add($"{DirectoryNameAdministrativeMetadata}/{AddmlXsdFileName}");
                break;
            case ArchiveType.SpecializedSystem:
                fileList.Add($"{DirectoryNameAdministrativeMetadata}/{AddmlXmlFileName}");
                fileList.Add($"{DirectoryNameAdministrativeMetadata}/{AddmlXsdFileName}");
                break;
            case ArchiveType.Siard:
                fileList.Add($"{DirectoryNameAdministrativeMetadata}/{SiardMetadataXmlFileName}");
                fileList.Add($"{DirectoryNameAdministrativeMetadata}/{SiardMetadataXsdFileName}");
                break;
        }

        return fileList.ToArray();
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
