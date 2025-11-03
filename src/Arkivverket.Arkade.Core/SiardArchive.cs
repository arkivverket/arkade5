using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.ExternalModels.Metadata;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core;

public class SiardArchive : Archive
{
    private readonly FileInfo _siardFile;

    public SiardArchive(FileInfo siardFile, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler) : base(processingDirectory)
    {
           _siardFile = siardFile;
           
        // TODO: Consider to handle the Siard-file and any external lobs in place (at least until packing)
        // CopySiardFilesToContentDirectory(siardFile, workingDirectory.Content().ToString());

        //ArchiveInformationEvent(archiveSource.FullName, archiveType);
        
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

    public SiardArchive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        _siardFile = inputDiasPackage.WorkingDirectory.ContentWorkDirectory().WithFile("*.siard");
    }

    public override bool IsTestable(out string disqualifyingCause)
    {
        if (!_siardFile.Exists)
        {
            disqualifyingCause = SiardMessages.CouldNotFindASiardFile;
            return false;
        }
        
        if (Details == null)
        {
            disqualifyingCause = SiardMessages.ValidatorDoesNotSupportVersionMessage;
            return false;
        }
        
        disqualifyingCause = null;
        return true;
    }
}
