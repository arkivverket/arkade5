using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class SpecializedSystemArchive : AddmlDefinitionTestedArchive
{
    
    [SetsRequiredMembers]
    public SpecializedSystemArchive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory)
        : base(new ArchiveContent(archiveExtractionDirectory), processingDirectory)
    {
    }

    [SetsRequiredMembers]
    public SpecializedSystemArchive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory)
        : base(new ArchiveContent(inputDiasPackage), processingDirectory, inputDiasPackage)
    {
    }
}
