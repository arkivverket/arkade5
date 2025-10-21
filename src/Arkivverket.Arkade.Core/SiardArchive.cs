using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;

namespace Arkivverket.Arkade.Core;

public class SiardArchive : Archive
{
    public SiardArchive(FileInfo siardFile, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler, InputDiasPackage inputDiasPackage) : base(null, processingDirectory, statusEventHandler, inputDiasPackage)
    {
        throw new NotImplementedException();
    }
}
