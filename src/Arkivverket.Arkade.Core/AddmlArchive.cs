using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core;

public class AddmlArchive(ArchiveType archiveType, FileSystemInfo archiveSource) : Archive(archiveType)
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
