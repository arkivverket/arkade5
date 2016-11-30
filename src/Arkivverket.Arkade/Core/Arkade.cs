using System.IO;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Report;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class Arkade
    {
        private readonly TestSessionFactory _testSessionFactory;

        public Arkade()
        {
            // TODO: Use autofac!
            _testSessionFactory = new TestSessionFactory(new TarCompressionUtility(), new ArchiveIdentifier(),
                new StatusEventHandler());
        }

        public TestSession RunTests(ArchiveDirectory archive)
        {
            TestSession testSession = _testSessionFactory.NewSessionFromArchiveDirectory(archive);
            return RunTests(testSession);
        }

        public TestSession RunTests(ArchiveFile archive)
        {
            TestSession testSession = _testSessionFactory.NewSessionFromArchiveFile(archive);
            return RunTests(testSession);
        }

        private TestSession RunTests(TestSession testSession)
        {
            // TODO: Use autofac!
            TestEngineFactory f =
                new TestEngineFactory(
                    new Noark5TestEngine(new ArchiveContentReader(), 
                    new TestProvider(new Noark5TestProvider(new ArchiveContentReader())),
                    new StatusEventHandler()),
                    new AddmlDatasetTestEngine(new FlatFileReaderFactory(), new AddmlProcessRunner(),
                        new StatusEventHandler()));

            ITestEngine testEngine = f.GetTestEngine(testSession);
            testSession.TestSuite = testEngine.RunTestsOnArchive(testSession);

            return testSession;
        }

        public bool SaveIp(TestSession testSession, DirectoryInfo directoryName)
        {
            if (!directoryName.Exists)
            {
                directoryName.Create();
            }
            string tarFile = Path.Combine(directoryName.FullName, testSession.Archive.Uuid.GetValue() + ".tar");
            TarCompressionUtility tar = new TarCompressionUtility();
            tar.CompressFolderContentToArchiveFile(tarFile, testSession.Archive.WorkingDirectory.FullName);


            var sourceInfoXml = new FileInfo(Path.Combine(testSession.Archive.WorkingDirectory.Parent.FullName,
                ArkadeConstants.InfoXmlFileName));
            if (sourceInfoXml.Exists)
            {
                string destinfoXml = Path.Combine(directoryName.FullName, ArkadeConstants.InfoXmlFileName);
                File.Copy(sourceInfoXml.FullName, destinfoXml, true);
            }

            return true;
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