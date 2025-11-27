using System.IO;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class Noark4Archive : Archive
{
    public Noark4Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory)
        : base(new ArchiveContent(archiveExtractionDirectory), processingDirectory)
    {
        ArchiveType = ArchiveType.Noark4; // TODO: Get rid of this ...
    }

    public Noark4Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory)
        : base(new ArchiveContent(ArchiveContent.GetContentDirectory(inputDiasPackage)), processingDirectory, inputDiasPackage)
    {
        ArchiveType = ArchiveType.Noark4; // TODO: Get rid of this ...
    }

    public override bool IsTestable(out string disqualifyingCause)
    {
        disqualifyingCause = Messages.Noark4ValidationNotSupported;

        return false;
    }
}
