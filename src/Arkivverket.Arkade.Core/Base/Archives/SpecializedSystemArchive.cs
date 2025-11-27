using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class SpecializedSystemArchive : AddmlDefinitionTestedArchive
{
    public SpecializedSystemArchive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory)
        : base(new ArchiveContent(archiveExtractionDirectory), processingDirectory)
    {
    }

    public SpecializedSystemArchive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory)
        : base(new ArchiveContent(ArchiveContent.GetContentDirectory(inputDiasPackage)), processingDirectory, inputDiasPackage)
    {
    }
}
