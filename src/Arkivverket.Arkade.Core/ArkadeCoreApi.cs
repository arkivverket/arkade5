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
using Serilog;

namespace Arkivverket.Arkade.Core;

/// <summary>Interact with the Arkade Core using Autofac.</summary>
public class ArkadeCoreApi(
    TestSessionFactory testSessionFactory,
    MetadataFilesCreator metadataFilesCreator,
    InformationPackageCreator informationPackageCreator,
    SiardMetadataFileHelper siardMetadataFileHelper,
    ICompressionUtility compressionUtility,
    IStatusEventHandler statusEventHandler,
    ArkadeApi arkadeApi)
{
    private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

    public Archive LoadArchiveExtraction(FileSystemInfo archiveSource, ArchiveType archiveType)
    {
        Log.Debug($"Loading Archive Extraction [sourcePath: {archiveSource.FullName}] [archiveType: {archiveType}]");

        ArchiveInformationEvent(archiveSource.FullName, archiveType);

        ArkadeDirectory content;

        if (archiveType == ArchiveType.Siard && archiveSource is FileInfo { Exists: true, Extension: ".siard" } siardFile)
        {
            // CopySiardFilesToContentDirectory(siardFile, workingDirectory.Content().ToString());

           throw new NotImplementedException();
        }
        else if (archiveSource is DirectoryInfo { Exists: true } directory)
        {
            content = new ArkadeDirectory(directory);
        }
        else
        {
            throw new ArkadeException(""); // TODO: ...
        }

        return new Archive(archiveType, content, statusEventHandler);
    }

    public Archive LoadArchiveAsDiasPackage(FileInfo diasPackageFile, ArchiveType archiveType, DirectoryInfo archiveProcessingDirectory)
    {
        Log.Debug($"Loading Dias Package [file: {diasPackageFile.FullName}] [archiveType: {archiveType}]");

        if (!Uuid.TryParse(Path.GetFileNameWithoutExtension(diasPackageFile.Name), out Uuid inputDiasPackageId)) // NB! UUID-orig
            throw new ArkadeException("Could not extract an UUID from filename: " + diasPackageFile.Name);
        
        ArchiveInformationEvent(diasPackageFile.FullName, archiveType, inputDiasPackageId);

        var diasPackageWorkingDirectory = new ArkadeDirectory(archiveProcessingDirectory.CreateSubdirectory(inputDiasPackageId.ToString()));

        //TarExtractionStartedEvent();
        compressionUtility.ExtractFolderFromArchive(diasPackageFile, diasPackageWorkingDirectory.DirectoryInfo(),
            withoutDocumentFiles: archiveType == ArchiveType.Noark5, archiveRootDirectoryName: inputDiasPackageId.ToString());
        //TarExtractionFinishedEvent(workingDirectory);

        ArkadeDirectory contentDirectory = diasPackageWorkingDirectory.WithSubDirectory(ArkadeConstants.DirectoryNameContent);

        var archive = new Archive(archiveType, contentDirectory, statusEventHandler, diasPackageFile.FullName);

        const PackageType packageType = PackageType.ArchivalInformationPackage; // Get ..
        var archiveMetadata = new ArchiveMetadata(); // Get ..
        
        archive.InputDiasPackage = new InputDiasPackage(inputDiasPackageId, packageType, archiveMetadata, archiveProcessingDirectory);
        
        return archive;
    }

    public TestSession CreateTestSession(Archive archive)
    {
        return testSessionFactory.NewSession(archive);
    }

    public void RunTests(Archive archive)
    {
        archive.TestSession.AddLogEntry(Messages.LogMessageStartTesting);

        Log.Information("Starting testing of archive.");

        LanguageManager.SetResourcesLanguageForTesting(archive.TestSession.OutputLanguage);

        if (archive.TestSession.TestRunContainsDocumentFileDependentTests)
            archive.DocumentFiles.Register(includeChecksums: archive.TestSession.TestRunContainsChecksumControl);

        ITestEngine testEngine = _testEngineFactory.GetTestEngine(testSession);
        testSession.TestSuite = testEngine.RunTestsOnArchive(testSession);

        testSession.AddLogEntry(Messages.LogMessageFinishedTesting);
        Log.Information("Testing of archive finished.");

        _testSessionXmlGenerator.GenerateXmlAndSaveToFile(testSession); // TODO: Is this file relevant any longer?
    }

    public string CreatePackage(Archive archive, SupportedLanguage language, bool generateFileFormatInfo, string outputDirectory)
    {
        string packageTypeAbbreviation = archive.OutputDiasPackage.PackageType.Equals(PackageType.SubmissionInformationPackage)
            ? "SIP"
            : "AIP";

        Log.Information($"Creating {packageTypeAbbreviation}.");

        LanguageManager.SetResourceLanguageForPackageCreation(language);

        if (generateFileFormatInfo)
        {
          arkadeApi.GenerateFileFormatInfoFiles(archive); // TODO: Integrate in ArkadeCoreApi
        }

        if (archive.ArchiveType is ArchiveType.Siard)
        {
            siardMetadataFileHelper.ExtractSiardMetadataFilesToAdministrativeMetadata(archive);
        }

        // metadataFilesCreator.Create(archive); // TODO: Handle!

        string packageFilePath;

        if (archive.OutputDiasPackage.PackageType == PackageType.SubmissionInformationPackage)
        {
            packageFilePath = informationPackageCreator.CreateSip(
                archive, outputDirectory
            );
        }
        else // ArchivalInformationPackage
        {
            packageFilePath = informationPackageCreator.CreateAip(
                archive, outputDirectory
            );
        }

        Log.Information($"{packageTypeAbbreviation} created at: {packageFilePath}");

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
