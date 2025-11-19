using System.IO;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class AddmlDefinitionTestedArchive(DirectoryInfo processingDirectory)
    : AddmlBasedArchive(processingDirectory)
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
