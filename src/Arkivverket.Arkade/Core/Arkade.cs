using System.IO;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Report;
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
            _testSessionFactory = new TestSessionFactory(new TarCompressionUtility(), new StatusEventHandler());
        }

        public TestSession RunTests(ArchiveFile archive)
        {
            TestSession testSession = _testSessionFactory.NewSession(archive);
            return RunTests(testSession);
        }

        private TestSession RunTests(TestSession testSession)
        {
            // TODO: Use autofac!
            TestEngineFactory f =
                new TestEngineFactory(
                    new Noark5TestEngine(new ArchiveContentReader(), 
                    new Noark5TestProvider(new ArchiveContentReader()),
                    new StatusEventHandler()),
                    new AddmlDatasetTestEngine(new FlatFileReaderFactory(), new AddmlProcessRunner(),
                        new StatusEventHandler()));

            ITestEngine testEngine = f.GetTestEngine(testSession);
            testSession.TestSuite = testEngine.RunTestsOnArchive(testSession);

            return testSession;
        }

        public void CreatePackage(TestSession testSession, PackageType packageType)
        {
            var informationPackageCreator = new InformationPackageCreator();
            if (packageType == PackageType.SubmissionInformationPackage)
            {
                informationPackageCreator.CreateSip(testSession.Archive);
            }
            else
            {
                informationPackageCreator.CreateAip(testSession.Archive);
            }
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