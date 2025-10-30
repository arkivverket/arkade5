using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core;

public class Noark4Archive(FileSystemInfo archiveSource, ICompressionUtility compressionUtility) : Archive(ArchiveType.Noark4, compressionUtility)
{
    public override bool IsTestable(out string disqualifyingCause)
    {
        disqualifyingCause = Messages.Noark4ValidationNotSupported;
        
        return false;
    }
}
