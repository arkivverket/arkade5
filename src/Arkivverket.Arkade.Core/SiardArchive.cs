using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.ExternalModels.Metadata;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core;

public class SiardArchive : Archive
{
    public SiardArchive(FileInfo siardFile, IStatusEventHandler statusEventHandler, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage) :
        base(ArchiveType.Siard, null, processingDirectory, inputDiasPackage)
    {
        FileInfo siardArchiveFile = Content.DirectoryInfo().GetFiles("*.siard").FirstOrDefault();
        if (siardArchiveFile == null)
            throw new ArkadeException("Siard file not found");
        if (!siardArchiveFile.Exists)
            throw new ArkadeException(string.Format(ExceptionMessages.FileNotFound, siardArchiveFile.FullName));

        if (new SiardArchiveReader().TryDeserializeToSiard2_1(siardArchiveFile.FullName, out siardArchive siard2Archive,
                out string errorMessage))
            Details = new SiardArchiveDetails(siard2Archive);
        else
        {
            statusEventHandler?.RaiseEventOperationMessage(null,
                string.Format(SiardMessages.DeserializationUnsuccessfulMessage, SiardMetadataXmlFileName, "2.1",
                    errorMessage),
                OperationMessageStatus.Error);

            Details = null;
        }
    }
}
