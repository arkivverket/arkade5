using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public interface IArchiveContent
{
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
        catch (DirectoryNotFoundException) // Part of the path not found
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
        catch (DirectoryNotFoundException) // Part of the path not found
        {
            return null;
        }
    }

    public IEnumerable<(FileSystemInfo Item, string RelativePath)> Get()
    {
        return RootDirectory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).Select(contentItem =>
            (contentItem, contentItem.FullName[RootDirectory.FullName.Length..].Replace('\\', '/')));
    }
}

public class FileArchiveContent(FileInfo contentFile) : IArchiveContent
{
    public FileInfo RootFile { get; } = contentFile;

    public IEnumerable<(FileSystemInfo Item, string RelativePath)> Get()
    {
        return [(RootFile, RootFile.Name)];
    }
}
