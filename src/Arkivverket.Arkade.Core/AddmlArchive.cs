using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core;

public class AddmlArchive(ArchiveType archiveType, FileSystemInfo archiveSource, ICompressionUtility compressionUtility) : Archive (archiveType, compressionUtility)
{
    public override bool IsTestable(out string disqualifyingCause)
    {
        if (TestSession.AddmlDefinition == null) // TODO: Follow up this
        {
            disqualifyingCause = Noark5Messages.CouldNotFindValidSpecificationFile;
            return false;
        }
        
        disqualifyingCause = null;
        return true;
    }   
}
