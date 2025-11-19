using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class SpecializedSystemArchive : Archive
{
    public SpecializedSystemArchive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        throw new NotImplementedException();
    }

    public SpecializedSystemArchive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        throw new NotImplementedException();
    }

    public override bool IsTestable(out string disqualifyingCause)
    {
        throw new NotImplementedException();
    }
}
