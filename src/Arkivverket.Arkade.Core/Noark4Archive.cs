using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core;

public class Noark4Archive(FileSystemInfo archiveSource) : Archive(ArchiveType.Noark4)
{
    public override bool IsTestable(out string disqualifyingCause)
    {
        disqualifyingCause = Messages.Noark4ValidationNotSupported;
        
        return false;
    }
}
