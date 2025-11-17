using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core;

public class SpecializedSystemArchive : Archive
{
    public SpecializedSystemArchive(DirectoryInfo extractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory)
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
