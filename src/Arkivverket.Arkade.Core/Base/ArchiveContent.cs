using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public interface IArchiveContent
{
    public IEnumerable<FileSystemInfo> GetAllContents();
}

public class DirectoryArchiveContent(DirectoryInfo contentDirectory) : IArchiveContent
{
    public DirectoryInfo RootDirectory { get; } = contentDirectory;

    public FileInfo GetFile(string filePath)
    {
        return RootDirectory.EnumerateFiles(filePath, SearchOption.AllDirectories).FirstOrDefault();
    }

    public DirectoryInfo GetDirectory(string directoryPath)
    {
        return RootDirectory.EnumerateDirectories(directoryPath, SearchOption.AllDirectories).FirstOrDefault();
    }

    public IEnumerable<FileSystemInfo> GetAllContents()
    {
        return RootDirectory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories);
    }
}

public class FileArchiveContent(FileInfo contentFile) : IArchiveContent
{
    public FileInfo RootFile { get; } = contentFile;

    public IEnumerable<FileSystemInfo> GetAllContents()
    {
        return [RootFile];
    }
}
