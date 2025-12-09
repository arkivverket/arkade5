using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class Noark3Archive(
    DirectoryArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage diasPackage = null)
    : AddmlDefinitionTestedArchive(content, processingDirectory, diasPackage);
