using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
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
        private readonly IArchiveTypeIdentifier _archiveTypeIdentifier;

        public ArkadeApi(TestSessionFactory testSessionFactory, TestEngineFactory testEngineFactory,
            MetadataFilesCreator metadataFilesCreator, InformationPackageCreator informationPackageCreator,
            TestSessionXmlGenerator testSessionXmlGenerator, IArchiveTypeIdentifier archiveTypeIdentifier)
        {
            _testSessionFactory = testSessionFactory;
            _testEngineFactory = testEngineFactory;
            _metadataFilesCreator = metadataFilesCreator;
            _informationPackageCreator = informationPackageCreator;
            _testSessionXmlGenerator = testSessionXmlGenerator;
            _archiveTypeIdentifier = archiveTypeIdentifier;
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

            ITestEngine testEngine = _testEngineFactory.GetTestEngine(testSession);
            testSession.TestSuite = testEngine.RunTestsOnArchive(testSession);

            testSession.AddLogEntry(Messages.LogMessageFinishedTesting);
            Log.Information("Testing of archive finished.");

            _testSessionXmlGenerator.GenerateXmlAndSaveToFile(testSession);
        }

        public string CreatePackage(TestSession testSession, string outputDirectory)
        {
            string packageType = testSession.ArchiveMetadata.PackageType.Equals(PackageType.SubmissionInformationPackage)
                ? "SIP"
                : "AIP";
            Log.Information($"Creating {packageType}.");

            LanguageManager.SetResourceLanguageForPackageCreation(testSession.OutputLanguage);

            _metadataFilesCreator.Create(testSession.Archive, testSession.ArchiveMetadata, testSession.GenerateFileFormatInfo);

            string packageFilePath;

            if (testSession.ArchiveMetadata.PackageType == PackageType.SubmissionInformationPackage)
            {
                packageFilePath = _informationPackageCreator.CreateSip(
                    testSession.Archive, testSession.ArchiveMetadata, outputDirectory
                );
            }
            else // ArchivalInformationPackage
            {
                packageFilePath = _informationPackageCreator.CreateAip(
                    testSession.Archive, testSession.ArchiveMetadata, outputDirectory
                );
            }

            Log.Information($"{packageType} created at: {packageFilePath}");

            return packageFilePath;
        }

        public void SaveReport(TestSession testSession, DirectoryInfo testReportDirectory, bool standalone)
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

            
            TestReportGeneratorRunner.RunAllGenerators(testSession, testReportDirectory, standalone);
        }

        public void GenerateFileFormatInfoFiles(DirectoryInfo filesDirectory, string resultFileDirectoryPath, string resultFileName, SupportedLanguage language)
        {
            LanguageManager.SetResourceLanguageForStandalonePronomAnalysis(language);
            
            string resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);

            FileFormatInfoGenerator.Generate(filesDirectory, resultFileFullName);
        }
        
        public Task<ArchiveFormatValidationReport> ValidateArchiveFormat(FileSystemInfo item, ArchiveFormat format, SupportedLanguage language)
        {
            LanguageManager.SetResourceLanguageForArchiveFormatValidation(language);

            return ArchiveFormatValidator.ValidateAsFormat(item, format);
        }

        public ArchiveType? DetectArchiveType(string archiveFileName)
        {
            return !Path.HasExtension(archiveFileName)
                ? _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(archiveFileName)
                : _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(archiveFileName);
        }
    }
}
