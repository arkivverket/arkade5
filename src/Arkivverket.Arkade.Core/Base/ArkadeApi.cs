using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

namespace Arkivverket.Arkade.Core.Base
{

    /// <summary>
    /// Use this class for interacting with the Arkade Api when you are using Autofac. If you don't use Autofac, please use the Arkade class instead.
    /// </summary>
    public class ArkadeApi
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly TestSessionFactory _testSessionFactory;
        private readonly TestEngineFactory _testEngineFactory;
        private readonly MetadataFilesCreator _metadataFilesCreator;
        private readonly InformationPackageCreator _informationPackageCreator;
        private readonly TestSessionXmlGenerator _testSessionXmlGenerator;
        private readonly SiardMetadataFileHelper _siardMetadataFileHelper;
        private readonly IArchiveTypeIdentifier _archiveTypeIdentifier;
        private readonly IArchiveFormatValidator _archiveFormatValidator;
        private readonly IFileFormatIdentifier _fileFormatIdentifier;
        private readonly IFileFormatInfoFilesGenerator _fileFormatInfoGenerator;
        private readonly ISiardXmlTableReader _siardXmlTableReader;
        private readonly MetadataExampleGenerator _metadataExampleGenerator;

        public ArkadeApi(TestSessionFactory testSessionFactory, TestEngineFactory testEngineFactory,
            MetadataFilesCreator metadataFilesCreator, InformationPackageCreator informationPackageCreator,
            TestSessionXmlGenerator testSessionXmlGenerator, SiardMetadataFileHelper siardMetadataFileHelper,
            IArchiveTypeIdentifier archiveTypeIdentifier, IArchiveFormatValidator archiveFormatValidator,
            IFileFormatIdentifier fileFormatIdentifier, IFileFormatInfoFilesGenerator fileFormatInfoGenerator, 
            ISiardXmlTableReader siardXmlTableReader, MetadataExampleGenerator metadataExampleGenerator)
        {
            _testSessionFactory = testSessionFactory;
            _testEngineFactory = testEngineFactory;
            _metadataFilesCreator = metadataFilesCreator;
            _informationPackageCreator = informationPackageCreator;
            _testSessionXmlGenerator = testSessionXmlGenerator;
            _siardMetadataFileHelper = siardMetadataFileHelper;
            _archiveTypeIdentifier = archiveTypeIdentifier;
            _archiveFormatValidator = archiveFormatValidator;
            _fileFormatIdentifier = fileFormatIdentifier;
            _fileFormatInfoGenerator = fileFormatInfoGenerator;
            _siardXmlTableReader = siardXmlTableReader;
            _metadataExampleGenerator = metadataExampleGenerator;
        }

        public TestSession RunTests(ArchiveDirectory archiveDirectory)
        {
            TestSession testSession = CreateTestSession(archiveDirectory);
            RunTests(testSession);
            return testSession;
        }

        public TestSession RunTests(ArchiveFile archive)
        {
            TestSession testSession = CreateTestSession(archive);
            RunTests(testSession);
            return testSession;
        }
        
        public TestSession CreateTestSession(ArchiveDirectory archiveDirectory)
        {
            return _testSessionFactory.NewSession(archiveDirectory);
        }

        public TestSession CreateTestSession(ArchiveFile archive)
        {
            return _testSessionFactory.NewSession(archive);
        }

        public void RunTests(TestSession testSession)
        {
            testSession.AddLogEntry(Messages.LogMessageStartTesting);

            Log.Information("Starting testing of archive.");

            LanguageManager.SetResourcesLanguageForTesting(testSession.OutputLanguage);

            if (testSession.TestRunContainsDocumentFileDependentTests)
                testSession.Archive.DocumentFiles.Register(includeChecksums: testSession.TestRunContainsChecksumControl);

            ITestEngine testEngine = _testEngineFactory.GetTestEngine(testSession);
            testSession.TestSuite = testEngine.RunTestsOnArchive(testSession);

            testSession.AddLogEntry(Messages.LogMessageFinishedTesting);
            Log.Information("Testing of archive finished.");

            _testSessionXmlGenerator.GenerateXmlAndSaveToFile(testSession);
        }

        public string CreatePackage(TestSession testSession, string outputDirectory)
        {
            testSession.Archive.NewUuid = Uuid.Random(); // NB! UUID-origin

            string packageType = testSession.Archive.Metadata.PackageType.Equals(PackageType.SubmissionInformationPackage)
                ? "SIP"
                : "AIP";
            Log.Information($"Creating {packageType}.");

            LanguageManager.SetResourceLanguageForPackageCreation(testSession.OutputLanguage);

            if (testSession.GenerateFileFormatInfo)
            {
                GenerateFileFormatInfoFiles(testSession);
            }

            if (testSession.Archive.ArchiveType is ArchiveType.Siard)
            {
                _siardMetadataFileHelper.ExtractSiardMetadataFilesToAdministrativeMetadata(testSession.Archive);
            }

            // Delete any existing dias-mets.xml extracted from input tar-file
            testSession.Archive.WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName).Delete();

            _metadataFilesCreator.Create(testSession.Archive, testSession.Archive.Metadata);

            string packageFilePath;

            if (testSession.Archive.Metadata.PackageType == PackageType.SubmissionInformationPackage)
            {
                packageFilePath = _informationPackageCreator.CreateSip(
                    testSession.Archive, testSession.Archive.Metadata, outputDirectory
                );
            }
            else // ArchivalInformationPackage
            {
                packageFilePath = _informationPackageCreator.CreateAip(
                    testSession.Archive, testSession.Archive.Metadata, outputDirectory
                );
            }

            Log.Information($"{packageType} created at: {packageFilePath}");

            return packageFilePath;
        }

        public void SaveReport(TestSession testSession, DirectoryInfo testReportDirectory, bool standalone, 
            int testResultDisplayLimit)
        {
            if(testReportDirectory.Exists)
                testReportDirectory.Delete(recursive: true);
            
            testReportDirectory.Create();

            if (testSession.Archive.ArchiveType == ArchiveType.Siard)
                File.Move(
                    sourceFileName: Path.Combine(testSession.Archive.WorkingDirectory.RepositoryOperations().ToString(),
                        OutputFileNames.DbptkValidationReportFile),
                    destFileName: Path.Combine(testReportDirectory.FullName, OutputFileNames.DbptkValidationReportFile)
                );

            
            TestReportGeneratorRunner.RunAllGenerators(testSession, testReportDirectory, standalone,
                testResultDisplayLimit);
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<byte>>> GetSiardLobsAsByteArrays(string siardFileFullPath)
        {
            return _siardXmlTableReader.CreateLobByteArrays(siardFileFullPath);
        }

        public IFileFormatInfo AnalyseFileFormat(FileInfo file)
        {
            return _fileFormatIdentifier.IdentifyFormat(file);
        }

        public IFileFormatInfo AnalyseFileFormat(KeyValuePair<string, IEnumerable<byte>> filePathAndByteContent)
        {
            return _fileFormatIdentifier.IdentifyFormat(filePathAndByteContent);
        }

        public IEnumerable<IFileFormatInfo> AnalyseFileFormats(string targetPath, FileFormatScanMode scanMode)
        {
            return _fileFormatIdentifier.IdentifyFormats(targetPath, scanMode);
        }
        
        public IEnumerable<IFileFormatInfo> AnalyseFileFormats(IEnumerable<KeyValuePair<string, IEnumerable<byte>>> filePathsAndByteContent)
        {
            return _fileFormatIdentifier.IdentifyFormats(filePathsAndByteContent);
        }
        
        public void GenerateFileFormatInfoFiles(TestSession testSession)
        {
            Archive archive = testSession.Archive;
            WorkingDirectory workingDirectory = archive.WorkingDirectory;
            try
            {
                var resultFileDirectoryPath = workingDirectory.AdministrativeMetadata().ToString();
                string resultFileName;
                string resultFileFullName;

                if (archive.ArchiveType == ArchiveType.Siard)
                {
                    string siardFileFullName = workingDirectory.Content().DirectoryInfo().GetFiles("*.siard")[0].FullName;

                    resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, Path.GetFileNameWithoutExtension(siardFileFullName));
                    resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);

                    IEnumerable<KeyValuePair<string, IEnumerable<byte>>> lobsAsByte = _siardXmlTableReader.CreateLobByteArrays(siardFileFullName);
                    _fileFormatIdentifier.BroadCastStarted();
                    IEnumerable<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobsAsByte);
                    _fileFormatIdentifier.BroadCastFinished();
                    _fileFormatInfoGenerator.Generate(formatAnalysedLobs, siardFileFullName, resultFileFullName);
                }
                else if (archive.IsNoark5TarArchive)
                {
                    IEnumerable<IFileFormatInfo> analysedTarContents = _fileFormatIdentifier
                        .IdentifyFormats(archive.ArchiveFileFullName, FileFormatScanMode.Archive).ToList();

                    string tarRootDirectoryName = Path.GetFileNameWithoutExtension(archive.ArchiveFileFullName);
                    string documentsDirectoryName = archive.GetDocumentsDirectoryName();

                    string tarFileRelativeDocumentsDirectoryPath = Path.Combine(tarRootDirectoryName!,
                        ArkadeConstants.DirectoryNameContent, documentsDirectoryName);

                    var fullDocumentsDirectoryTarPath = $"{archive.ArchiveFileFullName}#{tarFileRelativeDocumentsDirectoryPath}";

                    bool IsDocumentFile(IFileFormatInfo fileFormatInfo) => fileFormatInfo.FileName.StartsWith(fullDocumentsDirectoryTarPath);

                    IEnumerable<IFileFormatInfo> analysedDocumentFiles = analysedTarContents.Where(IsDocumentFile);

                    resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, documentsDirectoryName);
                    resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);

                    _fileFormatInfoGenerator.Generate(analysedDocumentFiles, tarFileRelativeDocumentsDirectoryPath, resultFileFullName);
                }
                else
                {
                    DirectoryInfo documentsDirectory = archive.GetDocumentsDirectory();
                    resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, documentsDirectory.Name);
                    resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);
                    IEnumerable<IFileFormatInfo> analysedFiles = _fileFormatIdentifier.IdentifyFormats(documentsDirectory.FullName, FileFormatScanMode.Directory);
                    _fileFormatInfoGenerator.Generate(analysedFiles, documentsDirectory.FullName, resultFileFullName);
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

        public void GenerateFileFormatInfoFiles(IEnumerable<IFileFormatInfo> fileFormatInfos, string relativePathRoot, string resultFileFullName, SupportedLanguage language)
        {
            LanguageManager.SetResourceLanguageForStandalonePronomAnalysis(language);

            _fileFormatInfoGenerator.Generate(fileFormatInfos, relativePathRoot, resultFileFullName);
        }

        public async Task<ArchiveFormatValidationReport> ValidateArchiveFormatAsync(
            FileSystemInfo item, ArchiveFormat format, string resultFileDirectoryPath, SupportedLanguage language)
        {
            LanguageManager.SetResourceLanguageForArchiveFormatValidation(language);

            return await _archiveFormatValidator.ValidateAsync(item, format, resultFileDirectoryPath);
        }

        public void GenerateMetadataExampleFile(string outputFileName)
        {
            _metadataExampleGenerator.Generate(outputFileName);
        }

        public ArchiveType? DetectArchiveType(string archiveFileName)
        {
            return !Path.HasExtension(archiveFileName)
                ? _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(archiveFileName)
                : _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(archiveFileName);
        }
    }
}
