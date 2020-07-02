using System.IO;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base
{

    /// <summary>
    /// Use this class for interacting with the Arkade Api when you are using Autofac. If you don't use Autofac, please use the Arkade class instead.
    /// </summary>
    public class ArkadeApi
    {
        private readonly TestSessionFactory _testSessionFactory;
        private readonly TestEngineFactory _testEngineFactory;
        private readonly MetadataFilesCreator _metadataFilesCreator;
        private readonly InformationPackageCreator _informationPackageCreator;
        private readonly TestSessionXmlGenerator _testSessionXmlGenerator;

        public ArkadeApi(TestSessionFactory testSessionFactory, TestEngineFactory testEngineFactory, MetadataFilesCreator metadataFilesCreator, InformationPackageCreator informationPackageCreator, TestSessionXmlGenerator testSessionXmlGenerator)
        {
            _testSessionFactory = testSessionFactory;
            _testEngineFactory = testEngineFactory;
            _metadataFilesCreator = metadataFilesCreator;
            _informationPackageCreator = informationPackageCreator;
            _testSessionXmlGenerator = testSessionXmlGenerator;
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

            ITestEngine testEngine = _testEngineFactory.GetTestEngine(testSession);
            testSession.TestSuite = testEngine.RunTestsOnArchive(testSession);

            testSession.AddLogEntry(Messages.LogMessageFinishedTesting);

            _testSessionXmlGenerator.GenerateXmlAndSaveToFile(testSession);
        }

        public string CreatePackage(TestSession testSession, string outputDirectory)
        {
            _metadataFilesCreator.Create(testSession.Archive, testSession.ArchiveMetadata, testSession.GenerateDocumentFileInfo);

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

            return packageFilePath;
        }

        public void SaveReport(TestSession testSession, FileInfo file)
        {
            using (FileStream fs = file.OpenWrite())
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    IReportGenerator reportGenerator = new HtmlReportGenerator(sw);
                    reportGenerator.Generate(testSession);
                }
            }
        }

    }
}
