using System.Diagnostics.CodeAnalysis;
using System.IO;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class Noark4Archive : Archive
{
    [SetsRequiredMembers]
    public Noark4Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory, GetContent(archiveExtractionDirectory))
    {
        ArchiveType = ArchiveType.Noark4; // TODO: Get rid of this ...
    }
    
    [SetsRequiredMembers]
    public Noark4Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory, GetContent(inputDiasPackage))
    {
        ArchiveType = ArchiveType.Noark4; // TODO: Get rid of this ...
    }

    public override bool IsTestable(out string disqualifyingCause)
    {
        disqualifyingCause = Messages.Noark4ValidationNotSupported;
        
        return false;
    }
}
