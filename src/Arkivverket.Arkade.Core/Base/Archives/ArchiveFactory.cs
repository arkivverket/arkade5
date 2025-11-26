using System;
using System.IO;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;
using static Arkivverket.Arkade.Core.Base.Archives.ArchiveType;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class ArchiveFactory(ICompressionUtility compressionUtility, IStatusEventHandler statusEventHandler)
{
    public Archive Create(FileSystemInfo archiveSource, ArchiveType archiveType)
    {
        DirectoryInfo processingDirectory = CreateProcessingDirectory();

        return (archiveType, archiveSource) switch
        {
            (ArchiveType.Siard, FileInfo { Extension: ".siard" } siardFile) =>
                new SiardArchive(siardFile, statusEventHandler, processingDirectory),
            (ArchiveType.Siard, FileInfo { Extension: ".tar" } tarFile) =>
                new SiardArchive(CreateInputDiasPackage(tarFile, processingDirectory), statusEventHandler, processingDirectory),

            (ArchiveType.Noark5, DirectoryInfo extractionDirectory) =>
                new Noark5Archive(extractionDirectory, processingDirectory),
            (ArchiveType.Noark5, FileInfo { Extension: ".tar" } tarFile) =>
                new Noark5Archive(CreateInputDiasPackage(tarFile, processingDirectory, true), processingDirectory),

            (Noark4, DirectoryInfo extractionDirectory) =>
                new Noark4Archive(extractionDirectory, processingDirectory),
            (Noark4, FileInfo { Extension: ".tar" } tarFile) =>
                new Noark4Archive(CreateInputDiasPackage(tarFile, processingDirectory), processingDirectory),

            (Noark3, DirectoryInfo extractionDirectory) =>
                new Noark3Archive(extractionDirectory, processingDirectory),
            (Noark3, FileInfo { Extension: ".tar" } tarFile) =>
                new Noark3Archive(CreateInputDiasPackage(tarFile, processingDirectory), processingDirectory),
            
            (SpecializedSystem, DirectoryInfo extractionDirectory) =>
                new SpecializedSystemArchive(extractionDirectory, processingDirectory),
            (SpecializedSystem, FileInfo { Extension: ".tar" } tarFile) =>
                new SpecializedSystemArchive(CreateInputDiasPackage(tarFile, processingDirectory), processingDirectory),

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
