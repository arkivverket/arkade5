using System.IO;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class Noark4Archive : Archive
{
    public new readonly DirectoryArchiveContent Content;
    
    public Noark4Archive(DirectoryArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage = null)
        : base(content, processingDirectory, inputDiasPackage)
    {
        ArchiveType = ArchiveType.Noark4; // TODO: Get rid of this ...
    }

    public override bool IsTestable(out string disqualifyingCause)
    {
        disqualifyingCause = Messages.Noark4ValidationNotSupported;

        return false;
    }
}
