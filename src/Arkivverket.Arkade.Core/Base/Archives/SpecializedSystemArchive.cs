using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class SpecializedSystemArchive(DirectoryArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage = null)
    : AddmlDefinitionTestedArchive(content, processingDirectory, inputDiasPackage);
