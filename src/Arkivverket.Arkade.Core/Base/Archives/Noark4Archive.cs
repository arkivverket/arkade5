using System.IO;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class Noark4Archive : Archive
{
    public Noark4Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        ArchiveType = ArchiveType.Noark4; // TODO: Get rid of this ...
        
        //Content = ...
    }
    
    public Noark4Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        ArchiveType = ArchiveType.Noark4; // TODO: Get rid of this ...
        
        Content = inputDiasPackage.WorkingDirectory.ContentWorkDirectory();
    }

    public override bool IsTestable(out string disqualifyingCause)
    {
        disqualifyingCause = Messages.Noark4ValidationNotSupported;
        
        return false;
    }
}
