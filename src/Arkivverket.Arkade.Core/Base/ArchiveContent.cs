using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveContent(FileSystemInfo[] contentItems)
{
    private FileSystemInfo[] ContentItems { get; } = contentItems;
    private DirectoryInfo RootDirectory { get; }
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
        return ContentItems.FirstOrDefault(f => f.FullName.EndsWith(filePath)) as FileInfo;
    }

    public DirectoryInfo GetDirectory(string directoryPath)
    {
        return ContentItems.FirstOrDefault(f => f.FullName.EndsWith(directoryPath)) as DirectoryInfo;
    }

    private static DirectoryInfo GetContentDirectory(InputDiasPackage diasPackage)
    {
        return diasPackage.WorkingDirectory.ContentWorkDirectory().DirectoryInfo();
    }
}
