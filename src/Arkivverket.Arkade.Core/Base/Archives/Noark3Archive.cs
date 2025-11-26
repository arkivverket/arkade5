using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class Noark3Archive : AddmlDefinitionTestedArchive
{
    [SetsRequiredMembers]
    public Noark3Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(new ArchiveContent(archiveExtractionDirectory), processingDirectory)
    {
    }

    [SetsRequiredMembers]
    public Noark3Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(new ArchiveContent(inputDiasPackage), processingDirectory, inputDiasPackage)
    {
    }
}
