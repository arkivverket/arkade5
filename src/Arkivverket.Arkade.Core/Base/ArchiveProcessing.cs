namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveProcessing(Archive archive)
    {
        public Archive Archive { get; } = archive;
        public InputDiasPackage InputDiasPackage { get; set; }
        public OutputDiasPackage OutputDiasPackage { get; set; }
        public TestSession TestSession { get; set; }
    }
}
