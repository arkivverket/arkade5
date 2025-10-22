using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;
using System.IO;

namespace Arkivverket.Arkade.Core;

public class Noark5Archive(DirectoryInfo contentDirectory, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler, InputDiasPackage inputDiasPackage) :
    Archive(ArchiveType.Noark5, contentDirectory, processingDirectory, statusEventHandler, inputDiasPackage);
