using System.IO;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class AddmlDefinitionTestedArchive(DirectoryInfo processingDirectory, ArkadeDirectory content)
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
