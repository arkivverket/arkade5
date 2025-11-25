using System.IO;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class AddmlDefinitionTestedArchive(DirectoryInfo processingDirectory, FileSystemInfo[] content)
    : AddmlBasedArchive(processingDirectory, content)
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
};
