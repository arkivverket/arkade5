using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core;

public class AddmlArchive(ArchiveType archiveType, DirectoryInfo contentDirectory, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage) :
    Archive(archiveType, contentDirectory, processingDirectory, inputDiasPackage)
{
    public override bool IsTestable(out string disqualifyingCause)
    {
        if(ArchiveType == ArchiveType.Noark4)
        {
            disqualifyingCause = Messages.Noark4ValidationNotSupported;
            return false;
        }
        
        if (TestSession.AddmlDefinition == null) // TODO: Follow up this
        {
            disqualifyingCause = Noark5Messages.CouldNotFindValidSpecificationFile;
            return false;
        }
        
        disqualifyingCause = null;
        return true;
    }   
}
