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
    TestEngineFactory testEngineFactory,
    TestSessionXmlGenerator testSessionXmlGenerator,
    MetadataFilesCreator metadataFilesCreator,
    InformationPackageCreator informationPackageCreator,
    SiardMetadataFileHelper siardMetadataFileHelper,
    ICompressionUtility compressionUtility,
    IStatusEventHandler statusEventHandler,
    ArkadeApi arkadeApi)
{
    private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

    public Archive LoadArchiveExtraction(FileSystemInfo archiveSource, ArchiveType archiveType) // Merge?
    {
        Log.Debug($"Loading Archive Extraction [sourcePath: {archiveSource.FullName}] [archiveType: {archiveType}]");

        ArchiveInformationEvent(archiveSource.FullName, archiveType);

        ArkadeDirectory archiveExtractionDirectory;

        if (archiveType == ArchiveType.Siard && archiveSource is FileInfo { Exists: true, Extension: ".siard" } siardFile)
        {
            // TODO: Consider to handle the Siard-file and any external lobs in place (at least until packing)
            // CopySiardFilesToContentDirectory(siardFile, workingDirectory.Content().ToString());

           throw new NotImplementedException();
        }
        else if (archiveSource is DirectoryInfo { Exists: true } directory)
        {
            archiveExtractionDirectory = new ArkadeDirectory(directory);
        }
        else
        {
            throw new ArkadeException(""); // TODO: ...
        }

        return new Archive(archiveType, archiveExtractionDirectory, CreateProcessingDirectory(), statusEventHandler);
    }

    public Archive LoadArchiveAsDiasPackage(FileInfo diasPackageFile, ArchiveType archiveType) // Merge?
    {
        Log.Debug($"Loading Dias Package [file: {diasPackageFile.FullName}] [archiveType: {archiveType}]");

        DirectoryInfo processingDirectory = CreateProcessingDirectory();

        var inputDiasPackage = new InputDiasPackage(diasPackageFile, archiveType, processingDirectory, compressionUtility); // DI for comporessionUtility?

        ArchiveInformationEvent(diasPackageFile.FullName, archiveType, inputDiasPackage.Id);
        
        var archive = new Archive(archiveType, inputDiasPackage, processingDirectory, statusEventHandler);
        
        return archive;
    }

    public TestSession CreateTestSession(Archive archive)
    {
        return testSessionFactory.NewSession(archive);
    }

    public void RunTests(Archive archive)
    {
        TestSession testSession = archive.TestSession;

        testSession.AddLogEntry(Messages.LogMessageStartTesting);

        Log.Information("Starting testing of archive.");

        LanguageManager.SetResourcesLanguageForTesting(testSession.OutputLanguage);

        if (testSession.TestRunContainsDocumentFileDependentTests)
            archive.DocumentFiles.Register(includeChecksums: testSession.TestRunContainsChecksumControl);

        ITestEngine testEngine = testEngineFactory.GetTestEngine(archive);
        testSession.TestSuite = testEngine.RunTestsOnArchive(archive);

        testSession.AddLogEntry(Messages.LogMessageFinishedTesting);
        Log.Information("Testing of archive finished.");

        testSessionXmlGenerator.GenerateXmlAndSaveToFile(archive); // TODO: Is this file relevant any longer?
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

        metadataFilesCreator.Create(archive); // TODO: Check!

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
    private DirectoryInfo CreateProcessingDirectory()
    {
        string workDirectoryFullName = ArkadeProcessingArea.WorkDirectory.FullName;
        var nowTimeStampString = DateTime.Now.ToString("yyyyMMddHHmmss");

        var processingDirectory = new DirectoryInfo(Path.Combine(workDirectoryFullName, nowTimeStampString));
        
        processingDirectory.Create();

        return processingDirectory;
    }
}
