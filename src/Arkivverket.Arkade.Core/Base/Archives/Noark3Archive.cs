using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class Noark3Archive : AddmlDefinitionTestedArchive
{
    public Noark3Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        throw new NotImplementedException();
    }

    public Noark3Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        throw new NotImplementedException();
    }
}
