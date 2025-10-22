using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;

namespace Arkivverket.Arkade.Core;

public class AddmlArchive(ArchiveType archiveType, DirectoryInfo contentDirectory, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler, InputDiasPackage inputDiasPackage) :
    Archive(archiveType, contentDirectory, processingDirectory, statusEventHandler, inputDiasPackage);
