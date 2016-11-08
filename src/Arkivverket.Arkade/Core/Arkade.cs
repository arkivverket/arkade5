using System.IO;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Report;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{

    // TODO: Should this be moved to test project?
    public class Arkade
    {
        private readonly TestSessionFactory _testSessionFactory;

        public Arkade()
        {
            // consider using Autofac for instantiating classes, see example in ConsoleTest-project
            _testSessionFactory = new TestSessionFactory(new TarCompressionUtility(), new ArchiveIdentifier(),
                new StatusEventHandler());
        }

        public TestSession RunTests(ArchiveFile archive)
        {
            TestSession testSession = _testSessionFactory.NewSessionFromArchive(archive);

            // TODO: Use autofac?
            TestEngineFactory f =
                new TestEngineFactory(
                    new TestEngine(new TestProvider(new Noark5TestProvider(new ArchiveContentReader())),
                        new StatusEventHandler()),
                    new AddmlDatasetTestEngine(new FlatFileReaderFactory(), new AddmlProcessRunner(), new StatusEventHandler()));

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

            string sourceInfoXml = Path.Combine(testSession.Archive.WorkingDirectory.Parent.FullName, ArkadeConstants.InfoXmlFileName);
            string destinfoXml = Path.Combine(directoryName.FullName, ArkadeConstants.InfoXmlFileName);
            File.Copy(sourceInfoXml, destinfoXml, true);

            return true;
        }

        public void SaveReport(TestSession testSession, FileInfo file)
        {
            PdfReportGenerator pdfReportGenerator = new PdfReportGenerator();
            PdfReport pdfReport = pdfReportGenerator.Generate(testSession);
            pdfReport.Save(file);
        }
    }
}
