using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class SpecializedSystemArchive : AddmlDefinitionTestedArchive
{
    
    [SetsRequiredMembers]
    public SpecializedSystemArchive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory)
        : base(SetupContent(archiveExtractionDirectory), processingDirectory)
    {
    }

    [SetsRequiredMembers]
    public SpecializedSystemArchive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory)
        : base(SetupContent(inputDiasPackage), processingDirectory)
    {
    }
}
