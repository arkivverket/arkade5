using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveContent(DirectoryInfo rootDirectory)
{
    public DirectoryInfo RootDirectory { get; } = rootDirectory;

    public FileInfo GetFile(string filePath)
    {
        return RootDirectory.EnumerateFiles(filePath, SearchOption.AllDirectories).FirstOrDefault();
    }

    public DirectoryInfo GetDirectory(string directoryPath)
    {
        return RootDirectory.EnumerateDirectories(directoryPath, SearchOption.AllDirectories).FirstOrDefault();
    }
}
