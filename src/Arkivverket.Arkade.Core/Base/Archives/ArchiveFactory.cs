using System;
using System.IO;
using Arkivverket.Arkade.Core.Logging;
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
            case DirectoryInfo directory: archiveContent = new DirectoryArchiveContent(directory);
                break;
            case FileInfo { Extension: ".siard" } siardFileInput: archiveContent = new FileArchiveContent(siardFileInput);
                break;
            case FileInfo { Extension: ".tar" } tarFile when CreateInputDiasPackage(tarFile, processingDirectory) is var diasPackage:
            {
                archiveContent = new DirectoryArchiveContent(diasPackage.GetContentDirectory());
                inputDiasPackage = diasPackage;
                break;
            }
            default: throw new ArgumentOutOfRangeException(nameof(archiveSource));
        }
        
        return (archiveType, archiveContent) switch
        {
            (ArchiveType.Siard, DirectoryArchiveContent or FileArchiveContent) => new SiardArchive(archiveContent, processingDirectory, statusEventHandler, inputDiasPackage),
            (ArchiveType.Noark5, DirectoryArchiveContent content) => new Noark5Archive(content, processingDirectory, inputDiasPackage),
            (ArchiveType.Noark4, DirectoryArchiveContent content) => new Noark4Archive(content, processingDirectory, inputDiasPackage),
            (ArchiveType.Noark3, DirectoryArchiveContent content) => new Noark3Archive(content, processingDirectory, inputDiasPackage),
            (ArchiveType.SpecializedSystem, DirectoryArchiveContent content) => new SpecializedSystemArchive(content, processingDirectory, inputDiasPackage),
            _ => throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null)
        };
    }

    private static DirectoryInfo CreateProcessingDirectory()
    {
        string workDirectoryFullName = ArkadeProcessingArea.WorkDirectory.FullName;
        var nowTimeStampString = DateTime.Now.ToString("yyyyMMddHHmmss");

        var processingDirectory = new DirectoryInfo(Path.Combine(workDirectoryFullName, nowTimeStampString));

        if(processingDirectory.Exists)
            throw new IOException("Processing directory already exists: " + processingDirectory.FullName);
        
        processingDirectory.Create();

        return processingDirectory;
    }

    private InputDiasPackage CreateInputDiasPackage(FileInfo tarFile, DirectoryInfo processingDirectory,
        bool extractWithoutDocumentFiles = false)
    {
        if (!Uuid.TryParse(Path.GetFileNameWithoutExtension(tarFile.Name), out Uuid id)) // NB! UUID-orig
            throw new ArkadeException("Could not extract an UUID from filename: " + tarFile.Name);

        DirectoryInfo workingDirectoryRoot = processingDirectory.CreateSubdirectory(id.GetValue());
        var diasPackageWorkingDirectory = new DiasPackageWorkingDirectory(workingDirectoryRoot);

        //TarExtractionStartedEvent();
            
        compressionUtility.ExtractFolderFromArchive(tarFile, diasPackageWorkingDirectory.Root().DirectoryInfo(),
            withoutDocumentFiles: extractWithoutDocumentFiles, archiveRootDirectoryName: id.ToString());
        //TarExtractionFinishedEvent(workingDirectory);

        var inputDiasPackage = new InputDiasPackage(id, diasPackageWorkingDirectory, tarFile);
        return inputDiasPackage;
    }
    
    //ArchiveInformationEvent(tarFile.FullName, archiveType, inputDiasPackage.Id);
    //ArchiveInformationEvent(archiveSource.FullName, archiveType);
    
    private void ArchiveInformationEvent(string archiveFileName, ArchiveType archiveType, Uuid inputDiasPackageUuid = null)
    {
        statusEventHandler.RaiseEventNewArchiveInformation(new ArchiveInformationEventArgs(
            archiveType.ToString(), inputDiasPackageUuid?.ToString() ?? "-",
            archiveFileName)); // NB! UUID-writeout (right after UUID init)
    }
}
