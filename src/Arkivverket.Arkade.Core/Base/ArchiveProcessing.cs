using Arkivverket.Arkade.Core.Languages;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveProcessing(Archive archive)
    {
        public Archive Archive { get; } = archive;

        public InformationPackage OriginalInformationPackage { get; set; }
        public TestSession TestSession { get; set; }
    }
}
