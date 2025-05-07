using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveProcessing
{
    public Archive Archive { get; }
    public InputDiasPackage InputDiasPackage { get; }
    public OutputDiasPackage OutputDiasPackage { get; set; }
    public TestSession TestSession { get; set; }
    public DirectoryInfo ProcessingDirectory { get; }

    public ArchiveProcessing(Archive archive)
    {
        Archive = archive;

        string workDirectory = ArkadeProcessingArea.WorkDirectory.FullName;
        var nowTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        ProcessingDirectory = new DirectoryInfo(Path.Combine(workDirectory, nowTimeStamp));
        ProcessingDirectory.Create();
    }

    public ArchiveProcessing(InputDiasPackage inputDiasPackage) : this(inputDiasPackage.Archive)
    {
        InputDiasPackage = inputDiasPackage;
    }
}
