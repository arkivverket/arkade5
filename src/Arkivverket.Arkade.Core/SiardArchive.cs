using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;

namespace Arkivverket.Arkade.Core;

public class SiardArchive : Archive
{
    public SiardArchive(FileInfo siardFile, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler, InputDiasPackage inputDiasPackage) :
        base(ArchiveType.Siard, null, processingDirectory, statusEventHandler, inputDiasPackage)
    {
    }
}
