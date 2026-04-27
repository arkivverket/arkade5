using System.IO;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class AddmlDefinitionTestedArchive(DirectoryArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage = null)
    : AddmlBasedArchive(content, processingDirectory, inputDiasPackage)
{
    public override bool IsTestable(out string disqualifyingCause)
    {
        if (AddmlXmlUnit == null)
        {
            disqualifyingCause = Noark5Messages.CouldNotFindValidSpecificationFile; // Move to some addml resource file
            return false;
        }

        disqualifyingCause = null;
        return true;
    }
}
