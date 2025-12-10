using System;
using System.IO;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.ExternalModels.Metadata;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class SiardArchive : Archive
{
    public readonly FileInfo SiardFile;

    public SiardArchive(IArchiveContent content, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler, InputDiasPackage inputDiasPackage = null)
        : base(content, processingDirectory, inputDiasPackage)
    {
        SiardFile = content switch
        {
            FileArchiveContent fileArchiveContent => fileArchiveContent.RootFile,
            DirectoryArchiveContent directoryArchiveContent => directoryArchiveContent.GetFile("*.siard"),
            _ => throw new ArgumentOutOfRangeException(nameof(content), content, null)
        };
        
        Details = GetArchiveDetails(SiardFile, statusEventHandler);
        
        // TODO: CopySiardFilesToContentDirectory handles external lobs (we don't just now) ...
        
    }

    private static SiardArchiveDetails GetArchiveDetails(FileInfo siardArchiveFile,
        IStatusEventHandler statusEventHandler)
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
