using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core;

public class AddmlArchive : Archive
{
    public AddmlArchive(ArchiveType archiveType, DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        Content = new ArkadeDirectory(archiveExtractionDirectory);
        
        ArchiveType = archiveType; // TODO: Get rid of this ...
    }
    
    public AddmlArchive(ArchiveType archiveType, InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        Content = inputDiasPackage.WorkingDirectory.ContentWorkDirectory();
        
        ArchiveType = archiveType; // TODO: Get rid of this ...
    }

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
