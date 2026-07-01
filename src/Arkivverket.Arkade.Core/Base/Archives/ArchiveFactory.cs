using System;
using System.IO;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class ArchiveFactory(ICompressionUtility compressionUtility, IStatusEventHandler statusEventHandler)
{
    public Archive Create(FileSystemInfo archiveSource, ArchiveType archiveType)
    {
        DirectoryInfo processingDirectory = CreateProcessingDirectory();

        IArchiveContent archiveContent;
        InputDiasPackage inputDiasPackage = null;

        switch (archiveSource)
        {
            case DirectoryInfo directory:
                archiveContent = new DirectoryArchiveContent(directory);
                break;
            case FileInfo { Extension: ".siard" } siardFileInput:
                archiveContent = new SiardFileArchiveContent(siardFileInput);
                break;
            case FileInfo { Extension: ".tar" } tarFile
                when CreateInputDiasPackage(tarFile, processingDirectory, archiveType) is var diasPackage:
            {
                archiveContent = new DirectoryArchiveContent(diasPackage.GetContentDirectory());
                inputDiasPackage = diasPackage;
                break;
            }
            default: throw new ArgumentOutOfRangeException(nameof(archiveSource));
        }

        return (archiveType, archiveContent) switch
        {
            (ArchiveType.Siard, SiardFileArchiveContent fileArchiveContent)
                => new SiardArchive(fileArchiveContent, processingDirectory, statusEventHandler),
            
            (ArchiveType.Siard, DirectoryArchiveContent directoryArchiveContent)
                => new SiardArchive(directoryArchiveContent, processingDirectory, statusEventHandler, inputDiasPackage),

            (ArchiveType.Noark5, DirectoryArchiveContent directoryArchiveContent)
                => new Noark5Archive(directoryArchiveContent, processingDirectory, inputDiasPackage),

            (ArchiveType.Noark4, DirectoryArchiveContent directoryArchiveContent)
                => new Noark4Archive(directoryArchiveContent, processingDirectory, inputDiasPackage),

            (ArchiveType.Noark3, DirectoryArchiveContent directoryArchiveContent)
                => new Noark3Archive(directoryArchiveContent, processingDirectory, inputDiasPackage),

            (ArchiveType.SpecializedSystem, DirectoryArchiveContent directoryArchiveContent)
                => new SpecializedSystemArchive(directoryArchiveContent, processingDirectory, inputDiasPackage),

            _ => throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null)
        };
    }

    private static DirectoryInfo CreateProcessingDirectory()
    {
        string workDirectoryFullName = ArkadeProcessingArea.WorkDirectory.FullName;
        var nowTimeStampString = DateTime.Now.ToString("yyyyMMddHHmmss");

        var processingDirectory = new DirectoryInfo(Path.Combine(workDirectoryFullName, nowTimeStampString));

        if (processingDirectory.Exists)
            throw new IOException("Processing directory already exists: " + processingDirectory.FullName);

        processingDirectory.Create();

        return processingDirectory;
    }

    private InputDiasPackage CreateInputDiasPackage(FileInfo tarFile, DirectoryInfo processingDirectory,
        ArchiveType archiveType)
    {
        if (!Uuid.TryParse(Path.GetFileNameWithoutExtension(tarFile.Name), out Uuid id)) // NB! UUID-orig
            throw new ArkadeException(string.Format(ExceptionMessages.FileNameUuidExtractionError, tarFile.Name));

        DirectoryInfo workingDirectoryRoot = processingDirectory.CreateSubdirectory(id.GetValue());
        var diasPackageWorkingDirectory = new DiasPackageWorkingDirectory(workingDirectoryRoot);

        statusEventHandler.RaiseEventOperationMessage(
            Messages.ReadingArchiveEvent, Messages.TarExtractionMessageStarted, OperationMessageStatus.Started);

        compressionUtility.ExtractFolderFromArchive(tarFile, diasPackageWorkingDirectory.Root().DirectoryInfo(),
            withoutDocumentFiles: archiveType == ArchiveType.Noark5, archiveRootDirectoryName: id.ToString());

        statusEventHandler.RaiseEventOperationMessage(
            Messages.ReadingArchiveEvent,
            string.Format(Messages.TarExtractionMessageFinished,
                diasPackageWorkingDirectory.ContentWorkDirectory().DirectoryInfo().FullName),
            OperationMessageStatus.Ok);

        var inputDiasPackage = new InputDiasPackage(id, diasPackageWorkingDirectory, tarFile);
        return inputDiasPackage;
    }
}
