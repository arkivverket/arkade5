using System.IO;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core;

public class AddmlArchive(ArchiveType archiveType, DirectoryInfo contentDirectory, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage) :
    Archive(archiveType, contentDirectory, processingDirectory, inputDiasPackage);
