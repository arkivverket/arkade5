using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using Serilog;

namespace Arkivverket.Arkade.Core.Base;

/// <summary>Interact with the Arkade Core using Autofac.</summary>
public class ArkadeCoreApi(
    TestSessionFactory testSessionFactory,
    TestEngineFactory testEngineFactory,
    TestSessionXmlGenerator testSessionXmlGenerator,
    InformationPackageCreator informationPackageCreator,
    ArchiveFactory archiveFactory,
    IArchiveTypeIdentifier archiveTypeIdentifier,
    IArchiveFormatValidator archiveFormatValidator,
    IFileFormatIdentifier fileFormatIdentifier,
    IFileFormatInfoFilesGenerator fileFormatInfoGenerator,
    ISiardXmlTableReader siardXmlTableReader,
    MetadataExampleGenerator metadataExampleGenerator)
{
    private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

    public Archive LoadArchiveExtraction(FileSystemInfo archiveSource, ArchiveType archiveType)
    {
        if(!archiveSource.Exists)
            throw new ArkadeException($"{archiveSource.FullName} was not found.");

        Log.Debug($"Loading Archive Extraction [sourcePath: {archiveSource.FullName}] [archiveType: {archiveType}]");

        return archiveFactory.Create(archiveSource, archiveType);
    }

    public ArchiveType? DetectArchiveType(string archiveFileName)
    {
        return !Path.HasExtension(archiveFileName)
            ? archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(archiveFileName)
            : archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(archiveFileName);
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

        if (archive is Noark5Archive noark5Archive && testSession.TestRunContainsDocumentFileDependentTests)
            noark5Archive.DocumentFiles.Register(includeChecksums: testSession.TestRunContainsChecksumControl);

        ITestEngine testEngine = testEngineFactory.GetTestEngine(archive);
        testSession.TestSuite = testEngine.RunTestsOnArchive(archive);

        testSession.AddLogEntry(Messages.LogMessageFinishedTesting);
        Log.Information("Testing of archive finished.");

        // TODO: Decide whether the arkade-log.xml test-session log is still needed in the package.
        // This is a pre-existing, product-level question (does anything downstream still consume it?)
        // and is unrelated to the model-renewal refactor that moved this call here.
        testSessionXmlGenerator.GenerateXmlAndSaveToFile(archive);
    }
    
    public DirectoryInfo GenerateTestReport(Archive archive, DirectoryInfo outputDirectory, int testResultDisplayLimit, DiasPackage diasPackage)
    {
        TestReportGeneratorRunner.RunAllGenerators(archive, outputDirectory, testResultDisplayLimit, diasPackage, out DirectoryInfo reportsDirectory);
        
        if (archive is SiardArchive)
            File.Copy(
                sourceFileName: Path.Combine(archive.TestSession.TemporaryTestResultFilesDirectory.FullName, OutputFileNames.DbptkValidationReportFile),
                destFileName: Path.Combine(reportsDirectory.FullName, OutputFileNames.DbptkValidationReportFile),
                overwrite: true
            );

        return reportsDirectory;
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
          GenerateFileFormatInfoFiles(archive);
        }

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

    public void GenerateFileFormatInfoFiles(Archive archive)
    {
        DiasPackageWorkingDirectory diasPackageWorkingDirectory = archive.OutputDiasPackage.WorkingDirectory;
        try
        {
            var resultFileDirectoryPath = diasPackageWorkingDirectory.AdministrativeMetadata().ToString();
            string resultFileName;
            string resultFileFullName;

            if (archive is SiardArchive siardArchive)
            {
                string siardFileFullName = siardArchive.SiardFile.FullName;

                resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, Path.GetFileNameWithoutExtension(siardFileFullName));
                resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);

                IEnumerable<KeyValuePair<string, IEnumerable<byte>>> lobsAsByte = siardXmlTableReader.CreateLobByteArrays(siardFileFullName);
                fileFormatIdentifier.BroadCastStarted();
                IEnumerable<IFileFormatInfo> formatAnalysedLobs = fileFormatIdentifier.IdentifyFormats(lobsAsByte);
                fileFormatIdentifier.BroadCastFinished();
                fileFormatInfoGenerator.Generate(formatAnalysedLobs, siardFileFullName, resultFileFullName);
            }
            else if (archive is Noark5Archive noark5Archive)
            {
                if (noark5Archive.SourceIsTarFile)
                {
                    IEnumerable<IFileFormatInfo> analysedTarContents = fileFormatIdentifier
                        .IdentifyFormats(noark5Archive.InputDiasPackage.TarFile.FullName, FileFormatScanMode.Archive)
                        .ToList();

                    string tarRootDirectoryName =
                        Path.GetFileNameWithoutExtension(noark5Archive.InputDiasPackage.TarFile.FullName);
                    string documentsDirectoryName = noark5Archive.GetDocumentsDirectoryName();

                    string tarFileRelativeDocumentsDirectoryPath = Path.Combine(tarRootDirectoryName!,
                        ArkadeConstants.DirectoryNameContent, documentsDirectoryName);

                    var fullDocumentsDirectoryTarPath =
                        $"{noark5Archive.InputDiasPackage.TarFile}#{tarFileRelativeDocumentsDirectoryPath}";

                    bool IsDocumentFile(IFileFormatInfo fileFormatInfo) =>
                        fileFormatInfo.FileName.StartsWith(fullDocumentsDirectoryTarPath);

                    IEnumerable<IFileFormatInfo> analysedDocumentFiles = analysedTarContents.Where(IsDocumentFile);

                    resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, documentsDirectoryName);
                    resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);

                    fileFormatInfoGenerator.Generate(analysedDocumentFiles, tarFileRelativeDocumentsDirectoryPath,
                        resultFileFullName);
                }
                else
                {
                    DirectoryInfo documentsDirectory = noark5Archive.GetDocumentsDirectory();
                    resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, documentsDirectory.Name);
                    resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);
                    IEnumerable<IFileFormatInfo> analysedFiles =
                        fileFormatIdentifier.IdentifyFormats(documentsDirectory.FullName,
                            FileFormatScanMode.Directory);
                    fileFormatInfoGenerator.Generate(analysedFiles, documentsDirectory.FullName,
                        resultFileFullName);
                }
            }
        }
        catch (SiegfriedFileFormatIdentifierException siegfriedException)
        {
            Log.Error(siegfriedException.Message);
        }
        catch (Exception e)
        {
            Log.Debug(e.ToString());
            Log.Error("An unforeseen error related to document file format analysis has occured. As a result, document file format analysis was aborted. Please see /arkade-tmp/logs for details.");
        }
    }

    public IEnumerable<IFileFormatInfo> AnalyseFileFormats(string targetPath, FileFormatScanMode scanMode)
    {
        return fileFormatIdentifier.IdentifyFormats(targetPath, scanMode);
    }

    public void GenerateFileFormatInfoFiles(IEnumerable<IFileFormatInfo> fileFormatInfos, string relativePathRoot, string resultFileFullName, SupportedLanguage language)
    {
        LanguageManager.SetResourceLanguageForStandalonePronomAnalysis(language);

        fileFormatInfoGenerator.Generate(fileFormatInfos, relativePathRoot, resultFileFullName);
    }

    public async Task<ArchiveFormatValidationReport> ValidateArchiveFormatAsync(
        FileSystemInfo item, ArchiveFormat format, string resultFileDirectoryPath, SupportedLanguage language)
    {
        // TODO: Resolve issues and re-enable PDF/A-validation
        if (format == ArchiveFormat.PdfA)
            throw new ArkadeException("Validation request with format PDF/A was rejected: 3rd party library issue");

        LanguageManager.SetResourceLanguageForArchiveFormatValidation(language);

        return await archiveFormatValidator.ValidateAsync(item, format, resultFileDirectoryPath);
    }

    public void GenerateMetadataExampleFile(string outputFileName)
    {
        metadataExampleGenerator.Generate(outputFileName);
    }
}