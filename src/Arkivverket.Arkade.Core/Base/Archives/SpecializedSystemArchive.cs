using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class SpecializedSystemArchive(DirectoryArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage = null)
    : AddmlDefinitionTestedArchive(content, processingDirectory, inputDiasPackage);
