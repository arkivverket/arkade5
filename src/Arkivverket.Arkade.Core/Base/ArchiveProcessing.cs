using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveProcessing(Archive archive)
{
    private Archive _archive = archive;
    private DirectoryInfo _processingDirectory;

    public DirectoryInfo ProcessingDirectory => _processingDirectory ?? CreateProcessingDirectory();

    private DirectoryInfo CreateProcessingDirectory()
    {
        string workDirectoryFullName = ArkadeProcessingArea.WorkDirectory.FullName;
        var nowTimeStampString = DateTime.Now.ToString("yyyyMMddHHmmss");

        _processingDirectory = new DirectoryInfo(Path.Combine(workDirectoryFullName, nowTimeStampString));
        _processingDirectory.Create();

        return _processingDirectory;
    }
}
