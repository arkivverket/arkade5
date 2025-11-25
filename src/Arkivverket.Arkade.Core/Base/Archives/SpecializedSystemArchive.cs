using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class SpecializedSystemArchive : AddmlDefinitionTestedArchive
{
    
    [SetsRequiredMembers]
    public SpecializedSystemArchive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory)
        : base(processingDirectory, SetupContent(archiveExtractionDirectory))
    {
    }

    [SetsRequiredMembers]
    public SpecializedSystemArchive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory)
        : base(processingDirectory, SetupContent(inputDiasPackage))
    {
    }
}
