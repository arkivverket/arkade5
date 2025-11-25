using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveContent(FileSystemInfo[] contentItems)
{
    private FileSystemInfo[] ContentItems { get; } = contentItems;
    
    public FileInfo GetFile(string fileName)
    {
        return ContentItems.FirstOrDefault(f => f.FullName.EndsWith(fileName)) as FileInfo;
    }

    public DirectoryInfo GetDirectory(string directoryName)
    {
        return ContentItems.FirstOrDefault(f => f.FullName.EndsWith(directoryName)) as DirectoryInfo;
    }
}