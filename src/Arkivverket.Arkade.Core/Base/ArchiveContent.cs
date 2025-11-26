using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveContent(FileSystemInfo[] contentItems)
{
    private FileSystemInfo[] ContentItems { get; } = contentItems;
    
    public FileInfo GetFile(string filePath)
    {
        return ContentItems.FirstOrDefault(f => f.FullName.EndsWith(filePath)) as FileInfo;
    }

    public DirectoryInfo GetDirectory(string directoryPath)
    {
        return ContentItems.FirstOrDefault(f => f.FullName.EndsWith(directoryPath)) as DirectoryInfo;
    }
}