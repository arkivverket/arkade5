using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public interface IArchiveContent
{
    public IEnumerable<(string FullPath, string RelativePath, bool IsDirectory)> FetchAll();
}

public class DirectoryArchiveContent(DirectoryInfo contentDirectory) : IArchiveContent
{
    public DirectoryInfo RootDirectory { get; } = contentDirectory;

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

    public IEnumerable<(string FullPath, string RelativePath, bool IsDirectory)> FetchAll()
    {
        foreach (FileSystemInfo contentItem in RootDirectory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
            yield return (contentItem.FullName, GetRelativePath(contentItem), IsDirectory(contentItem));
        yield break;

        string GetRelativePath(FileSystemInfo fileSystemInfo)
        {
            int offset = Path.EndsInDirectorySeparator(RootDirectory.FullName)
                ? RootDirectory.FullName.Length
                : RootDirectory.FullName.Length + 1;

            return fileSystemInfo.FullName[offset..].Replace('\\', '/');
        }

        bool IsDirectory(FileSystemInfo fileSystemInfo)
        {
            return fileSystemInfo.Attributes.HasFlag(FileAttributes.Directory);
        }
    }
}

public class FileArchiveContent(FileInfo contentFile) : IArchiveContent
{
    public FileInfo RootFile { get; } = contentFile;

    public IEnumerable<(string FullPath, string RelativePath, bool IsDirectory)> FetchAll()
    {
        yield return (RootFile.FullName, RootFile.Name, false);
    }
}
