using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveContent(FileSystemInfo[] contentItems)
{
    private FileSystemInfo[] ContentItems { get; } = contentItems;
    public DirectoryInfo RootDirectory { get; }
    private InputDiasPackage InputDiasPackage { get; } // TODO: Consider if needed (for content tar-entry operations)

    public ArchiveContent(DirectoryInfo contentDirectory) : this(contentDirectory.GetFileSystemInfos())
    {
        RootDirectory = contentDirectory;
    }

    public ArchiveContent(InputDiasPackage inputDiasPackage) : this(GetContentDirectory(inputDiasPackage))
    {
        InputDiasPackage = inputDiasPackage;
    }
    
    public FileInfo GetFile(string filePath)
    {
        return RootDirectory.EnumerateFiles(filePath, SearchOption.AllDirectories).FirstOrDefault();
    }

    public DirectoryInfo GetDirectory(string directoryPath)
    {
        return RootDirectory.EnumerateDirectories(directoryPath, SearchOption.AllDirectories).FirstOrDefault();
    }

    private static DirectoryInfo GetContentDirectory(InputDiasPackage diasPackage)
    {
        return diasPackage.WorkingDirectory.ContentWorkDirectory().DirectoryInfo();
    }
}
