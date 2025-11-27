using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class Noark3Archive : AddmlDefinitionTestedArchive
{
    public Noark3Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory)
        : base(new ArchiveContent(archiveExtractionDirectory), processingDirectory)
    {
    }

    public Noark3Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory)
        : base(new ArchiveContent(ArchiveContent.GetContentDirectory(inputDiasPackage)), processingDirectory, inputDiasPackage)
    {
    }
}
