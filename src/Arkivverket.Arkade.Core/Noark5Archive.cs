using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;
using System;
using System.IO;

namespace Arkivverket.Arkade.Core;

public class Noark5Archive : Archive
{
    public Noark5Archive(DirectoryInfo contentDirectory, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler, InputDiasPackage inputDiasPackage) : base(null, processingDirectory, statusEventHandler, inputDiasPackage)
    {
        throw new NotImplementedException();
    }
}
