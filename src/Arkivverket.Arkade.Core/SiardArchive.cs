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
    public readonly FileInfo SiardFile;

    public SiardArchive(FileInfo siardFile, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler) : base(processingDirectory)
    {
        ArchiveType = ArchiveType.Siard; // TODO: Get rid of this ...

        //Content = ...
        
        SiardFile = siardFile;
           
        // TODO: Consider to handle the Siard-file and any external lobs in place (at least until packing)
        // CopySiardFilesToContentDirectory(siardFile, workingDirectory.Content().ToString());

        //ArchiveInformationEvent(archiveSource.FullName, archiveType);
        
        Details = GetArchiveDetails(siardFile, statusEventHandler);
    }
    
    public SiardArchive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler) : base(processingDirectory)
    {
        ArchiveType = ArchiveType.Siard; // TODO: Get rid of this ...
        
        Content = inputDiasPackage.WorkingDirectory.ContentWorkDirectory();
        
        FileInfo siardFile = Content.DirectoryInfo().GetFiles("*.siard").FirstOrDefault();

        SiardFile = siardFile ?? throw new ArkadeException("Siard file not found");
        
        Details = GetArchiveDetails(siardFile, statusEventHandler);
    }
    
    private static SiardArchiveDetails GetArchiveDetails(FileInfo siardArchiveFile, IStatusEventHandler statusEventHandler)
    {
        if (new SiardArchiveReader().TryDeserializeToSiard2_1(
                siardArchiveFile.FullName, out siardArchive siard2Archive, out string errorMessage))
            return new SiardArchiveDetails(siard2Archive);

        statusEventHandler?.RaiseEventOperationMessage(null, string.Format(
            SiardMessages.DeserializationUnsuccessfulMessage, SiardMetadataXmlFileName,
            "2.1", errorMessage), OperationMessageStatus.Error);

        return null;
    }

    public override bool IsTestable(out string disqualifyingCause)
    {
        if (!SiardFile.Exists)
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
