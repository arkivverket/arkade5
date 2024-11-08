namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveProcessing(Archive archive)
    {
        public Archive Archive { get; } = archive;

        public DiasPackage OriginalDiasPackage { get; set; }
        public TestSession TestSession { get; set; }
    }
}
