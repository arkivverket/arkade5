using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveProcessing(Archive archive)
{
    public ArchiveProcessing(InputDiasPackage inputDiasPackage) : this(inputDiasPackage.Archive)
    {
        InputDiasPackage = inputDiasPackage;
    }

    public Archive Archive { get; } = archive;
    public InputDiasPackage InputDiasPackage { get; }
    public OutputDiasPackage OutputDiasPackage { get; set; }
    public TestSession TestSession { get; set; }
    public DirectoryInfo ProcessingDirectory => _processingDirectory ?? CreateProcessingDirectory();

    private DirectoryInfo _processingDirectory;

    private DirectoryInfo CreateProcessingDirectory()
    {
        string workDirectoryFullName = ArkadeProcessingArea.WorkDirectory.FullName;
        var nowTimeStampString = DateTime.Now.ToString("yyyyMMddHHmmss");

        _processingDirectory = new DirectoryInfo(Path.Combine(workDirectoryFullName, nowTimeStampString));
        _processingDirectory.Create();

        return _processingDirectory;
    }
}
