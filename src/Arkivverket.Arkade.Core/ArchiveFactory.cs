using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core;

public class ArchiveFactory(ICompressionUtility compressionUtility, IStatusEventHandler statusEventHandler)
{
    public Archive Create(FileSystemInfo archiveSource, ArchiveType archiveType)
    {
        return archiveType switch
        {
            ArchiveType.Siard => CreateSiardArchive(archiveSource),
            ArchiveType.Noark5 => CreateNoark5Archive(archiveSource),
            ArchiveType.Noark4 => CreateNoark4Archive(archiveSource),
            ArchiveType.Noark3 or ArchiveType.Fagsystem => CreateAddmlArchive(archiveSource, archiveType),
            _ => throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null)
        };
    }

    private SiardArchive CreateSiardArchive(FileSystemInfo archiveSource)
    {
        DirectoryInfo processingDirectory = CreateProcessingDirectory();
        
        if (IsTarFile(archiveSource, out FileInfo tarFile))
        {
            InputDiasPackage inputDiasPackage = CreateInputDiasPackage(tarFile, processingDirectory);

            return new SiardArchive(inputDiasPackage, processingDirectory, statusEventHandler);
        }

        if (archiveSource is FileInfo { Extension: ".siard" } siardFile)
        {
            return new SiardArchive(siardFile, processingDirectory, statusEventHandler);
        }

        throw new ArkadeException(
            $"{archiveSource.FullName} was not recognized as a Siard archive file.");
    }

    private Noark5Archive CreateNoark5Archive(FileSystemInfo archiveSource)
    {
        DirectoryInfo processingDirectory = CreateProcessingDirectory();

        if (IsTarFile(archiveSource, out FileInfo tarFile))
        {
            InputDiasPackage inputDiasPackage = CreateInputDiasPackage(tarFile, processingDirectory, true);

            return new Noark5Archive(inputDiasPackage, processingDirectory);
        }
        
        var archiveExtractionDirectory = archiveSource as DirectoryInfo;

        return new Noark5Archive(archiveExtractionDirectory, processingDirectory);
    }

    private Noark4Archive CreateNoark4Archive(FileSystemInfo archiveSource)
    {
        DirectoryInfo processingDirectory = CreateProcessingDirectory();

        if (IsTarFile(archiveSource, out FileInfo tarFile))
        {
            InputDiasPackage inputDiasPackage = CreateInputDiasPackage(tarFile, processingDirectory, true);

            return new Noark4Archive(inputDiasPackage, processingDirectory);
        }
        
        var archiveExtractionDirectory = archiveSource as DirectoryInfo;

        return new Noark4Archive(archiveExtractionDirectory, processingDirectory);
    }

    private AddmlArchive CreateAddmlArchive(FileSystemInfo archiveSource, ArchiveType archiveType)
    {
        DirectoryInfo processingDirectory = CreateProcessingDirectory();

        if (IsTarFile(archiveSource, out FileInfo tarFile))
        {
            InputDiasPackage inputDiasPackage = CreateInputDiasPackage(tarFile, processingDirectory);

            return new AddmlArchive(archiveType, inputDiasPackage, processingDirectory);
        }
        
        var archiveExtractionDirectory = archiveSource as DirectoryInfo;

        return new AddmlArchive(archiveType, archiveExtractionDirectory, processingDirectory);
    }

    private static DirectoryInfo CreateProcessingDirectory()
    {
        string workDirectoryFullName = ArkadeProcessingArea.WorkDirectory.FullName;
        var nowTimeStampString = DateTime.Now.ToString("yyyyMMddHHmmss");

        var processingDirectory = new DirectoryInfo(Path.Combine(workDirectoryFullName, nowTimeStampString));

        processingDirectory.Create();

        return processingDirectory;
    }

    private static bool IsTarFile(FileSystemInfo archiveSource, out FileInfo file)
    {
        if (archiveSource is FileInfo { Extension: ".tar" } tarFile)
        {
            file = tarFile;
            return true;
        }

        file = null;
        return false;
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
