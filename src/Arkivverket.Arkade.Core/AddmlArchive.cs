using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;

namespace Arkivverket.Arkade.Core;

public class AddmlArchive : Archive
{
    public AddmlArchive(DirectoryInfo contentDirectory, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler, InputDiasPackage inputDiasPackage) : base(null, processingDirectory, statusEventHandler, inputDiasPackage)
    {
        throw new NotImplementedException();
    }
}
