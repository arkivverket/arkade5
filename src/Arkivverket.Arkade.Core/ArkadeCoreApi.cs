using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using Serilog;

namespace Arkivverket.Arkade.Core;

/// <summary>Interact with the Arkade Core using Autofac.</summary>
public class ArkadeCoreApi(
    TestSessionFactory testSessionFactory,
    MetadataFilesCreator metadataFilesCreator,
    InformationPackageCreator informationPackageCreator,
    SiardMetadataFileHelper siardMetadataFileHelper,
    ICompressionUtility compressionUtility,
    IStatusEventHandler statusEventHandler)
{
    private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

    public Archive LoadArchiveExtraction(FileSystemInfo archiveSource, ArchiveType archiveType)
    {
        Log.Debug($"Loading Archive Extraction [sourcePath: {archiveSource.FullName}] [archiveType: {archiveType}]");

        ArchiveInformationEvent(archiveSource.FullName, archiveType);

        WorkingDirectory workingDirectory;
        ArkadeDirectory content;

        if (archiveType == ArchiveType.Siard && archiveSource is FileInfo { Exists: true, Extension: ".siard" } siardFile)
        {
            workingDirectory = WorkingDirectory.FromArchiveFile();

           // CopySiardFilesToContentDirectory(siardFile, workingDirectory.Content().ToString());

           throw new NotImplementedException();
        }
        else if (archiveSource is DirectoryInfo { Exists: true } directory)
        {
            content = new ArkadeDirectory(directory);
            
            workingDirectory = WorkingDirectory.FromExternalDirectory(directory);
        }
        else
        {
            throw new ArkadeException(""); // TODO: ...
        }

        

        
        
        return new Archive(archiveType, workingDirectory, content, statusEventHandler);
    }

    public InputDiasPackage LoadDiasPackage(FileInfo diasPackage, ArchiveType archiveType)
    {
        Log.Debug($"Loading Dias Package [file: {diasPackage.FullName}] [archiveType: {archiveType}]");

        Uuid.TryParse(Path.GetFileNameWithoutExtension(diasPackage.Name), out Uuid inputDiasPackageId); // NB! UUID-orig

        ArchiveInformationEvent(diasPackage.FullName, archiveType, inputDiasPackageId);

        ArchiveProcessing archiveProcessing = null; // TODO: Provide

        var workingDirectory = new WorkingDirectory(archiveProcessing.ProcessingDirectory);
        
        //else
        {
            //TarExtractionStartedEvent();
            compressionUtility.ExtractFolderFromArchive(diasPackage, workingDirectory.Root().DirectoryInfo(),
                withoutDocumentFiles: archiveType == ArchiveType.Noark5, archiveRootDirectoryName: inputDiasPackageId?.ToString());
            //TarExtractionFinishedEvent(workingDirectory);
        }

        ArkadeDirectory content = null; // TODO Get ...

        var archive = new Archive(archiveType, workingDirectory, content, statusEventHandler, diasPackage.FullName);

        const PackageType packageType = PackageType.ArchivalInformationPackage; // Get ..
        var archiveMetadata = new ArchiveMetadata(); // Get ..

        return new InputDiasPackage(inputDiasPackageId, packageType, archive, archiveMetadata);
    }

    public TestSession CreateTestSession(Archive archive)
    {
        return testSessionFactory.NewSession(archive);
    }

    public string CreatePackage(OutputDiasPackage diasPackage, SupportedLanguage language, bool generateFileFormatInfo, string outputDirectory)
    {
        string packageType = diasPackage.PackageType.Equals(PackageType.SubmissionInformationPackage)
            ? "SIP"
            : "AIP";

        Log.Information($"Creating {packageType}.");

        LanguageManager.SetResourceLanguageForPackageCreation(language);

        if (generateFileFormatInfo)
        {
          // GenerateFileFormatInfoFiles(diasPackage.Archive);
        }

        if (diasPackage.Archive.ArchiveType is ArchiveType.Siard)
        {
            siardMetadataFileHelper.ExtractSiardMetadataFilesToAdministrativeMetadata(diasPackage.Archive);
        }

        // Delete any existing dias-mets.xml extracted from input tar-file
        //diasPackage.Archive.Content.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName).Delete();

        metadataFilesCreator.Create(diasPackage);

        string packageFilePath;

        if (diasPackage.PackageType == PackageType.SubmissionInformationPackage)
        {
            packageFilePath = informationPackageCreator.CreateSip(
                diasPackage, outputDirectory
            );
        }
        else // ArchivalInformationPackage
        {
            packageFilePath = informationPackageCreator.CreateAip(
                diasPackage, outputDirectory
            );
        }

        Log.Information($"{packageType} created at: {packageFilePath}");

        return packageFilePath;
    }

    private void ArchiveInformationEvent(string archiveFileName, ArchiveType archiveType, Uuid inputDiasPackageUuid = null)
    {
        statusEventHandler.RaiseEventNewArchiveInformation(new ArchiveInformationEventArgs(
            archiveType.ToString(), inputDiasPackageUuid?.ToString() ?? "-", archiveFileName)); // NB! UUID-writeout (right after UUID init)
    }

    private void CopySiardFilesToContentDirectory(FileInfo siardArchiveFile, string contentDirectoryPath)
    {
        var siardTableXmlReader = new SiardXmlTableReader(new SiardArchiveReader());

        siardArchiveFile.CopyTo(Path.Combine(contentDirectoryPath, siardArchiveFile.Name));

        try
        {
            IEnumerable<string> fullPathsToExternalLobs =
                siardTableXmlReader.GetFullPathsToExternalLobs(siardArchiveFile.FullName);

            foreach (string fullPathToExternalLob in fullPathsToExternalLobs)
            {
                if (!File.Exists(fullPathToExternalLob))
                {
                    string message = string.Format(SiardMessages.ExternalLobFileNotFoundMessage, fullPathToExternalLob);
                    statusEventHandler.RaiseEventOperationMessage("", message, OperationMessageStatus.Error);
                    Log.Error(message);
                    continue;
                }

                string relativePathFromSiardFileToExternalLob =
                    Path.GetRelativePath(siardArchiveFile.DirectoryName, fullPathToExternalLob);

                string externalLobDestinationPath =
                    Path.Combine(contentDirectoryPath, relativePathFromSiardFileToExternalLob);

                var destinationDirectoryForExternalLob = Path.GetDirectoryName(externalLobDestinationPath);

                Directory.CreateDirectory(destinationDirectoryForExternalLob);

                File.Copy(fullPathToExternalLob, externalLobDestinationPath);

                Log.Debug("'{0}' has been added to Arkade temporary work area", fullPathToExternalLob);
            }
        }
        catch (SiardArchiveReaderException)
        {
            statusEventHandler.RaiseEventOperationMessage("", SiardMessages.ExternalLobsNotCopiedWarning, OperationMessageStatus.Warning);
        }
    }

}
