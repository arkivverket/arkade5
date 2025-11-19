using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class SpecializedSystemArchive : AddmlDefinitionTestedArchive
{
    public SpecializedSystemArchive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        throw new NotImplementedException();
    }

    public SpecializedSystemArchive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        throw new NotImplementedException();
    }
}
