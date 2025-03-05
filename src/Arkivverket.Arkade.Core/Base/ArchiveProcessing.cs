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
}
