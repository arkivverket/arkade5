using System.Collections.Generic;
using System.Linq;
using System.IO;
using Arkivverket.Arkade.Core.Base.Siard;
using Serilog;

namespace Arkivverket.Arkade.Core.Base;

public interface IArchiveContent
{
    public IEnumerable<(FileInfo File, string RelativePath)> GetFiles();
    public IEnumerable<(FileSystemInfo Item, string RelativePath)> Get();
}

public class DirectoryArchiveContent(DirectoryInfo contentDirectory) : IArchiveContent
{
    public DirectoryInfo RootDirectory { get; } = new(contentDirectory.FullName.TrimEnd('\\', '/') + '/'); //Ensures '/'

    public FileInfo GetFile(string filePath)
    {
        try
        {
            return RootDirectory.EnumerateFiles(filePath).FirstOrDefault();
        }
        catch (DirectoryNotFoundException)
        {
            return null;
        }
    }

    public DirectoryInfo GetDirectory(string directoryPath)
    {
        try
        {
            return RootDirectory.EnumerateDirectories(directoryPath).FirstOrDefault();
        }
        catch (DirectoryNotFoundException)
        {
            return null;
        }
    }

    public IEnumerable<(FileInfo File, string RelativePath)> GetFiles()
    {
        return RootDirectory.EnumerateFiles("*", SearchOption.AllDirectories)
            .Select(contentFile => (contentFile, GetRelativePath(contentFile)));
    }

    public IEnumerable<(FileSystemInfo Item, string RelativePath)> Get()
    {
        return RootDirectory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories)
            .Select(contentItem => (contentItem, GetRelativePath(contentItem)));
    }

    private string GetRelativePath(FileSystemInfo contentItem)
    {
        return contentItem.FullName[RootDirectory.FullName.Length..].Replace('\\', '/');
    }
}

public class FileArchiveContent(FileInfo contentFile) : IArchiveContent
{
    public FileInfo RootFile { get; } = contentFile;

    public virtual IEnumerable<(FileInfo File, string RelativePath)> GetFiles()
    {
        return [(File: RootFile, RelativePath: RootFile.Name)];
    }

    public virtual IEnumerable<(FileSystemInfo Item, string RelativePath)> Get()
    {
        return [(RootFile, RootFile.Name)];
    }
}

public class SiardFileArchiveContent(FileInfo siardFile) : FileArchiveContent(siardFile)
{
    private readonly SiardXmlTableReader _siardTableXmlReader = new(new SiardArchiveReader());

    public override IEnumerable<(FileInfo File, string RelativePath)> GetFiles()
    {
        FileInfo siardFile = RootFile;

        IEnumerable<string> externalLobsFullPaths = _siardTableXmlReader.GetFullPathsToExternalLobs(siardFile.FullName);

        ILookup<bool, string> lobsFullPathsGroupedByFileExistence = externalLobsFullPaths.ToLookup(File.Exists);
        IEnumerable<string> existingLobsFullPaths = lobsFullPathsGroupedByFileExistence[true];
        IEnumerable<string> missingLobsFullPaths = lobsFullPathsGroupedByFileExistence[false];

        int numberOfMissingLobs = missingLobsFullPaths.Count();
        if (numberOfMissingLobs > 0)
            Log.Error("{NumberOfMissingLobs} external lobs could not be found", numberOfMissingLobs);
        
        int siardFileLocationPathLength = siardFile.DirectoryName!.TrimEnd(Path.DirectorySeparatorChar).Length + 1;
        
        IEnumerable<(FileInfo, string)> externalLobsTuples = existingLobsFullPaths.Select(lobFullPath
            => (new FileInfo(lobFullPath), lobFullPath[siardFileLocationPathLength..].Replace('\\', '/')));

        (FileInfo siardFile, string Name) siardFileTuple = (siardFile, siardFile.Name);

        return [siardFileTuple, ..externalLobsTuples];
    }

    public override IEnumerable<(FileSystemInfo Item, string RelativePath)> Get()
        => GetFiles().Select(t => ((FileSystemInfo)t.File, t.RelativePath));
}
