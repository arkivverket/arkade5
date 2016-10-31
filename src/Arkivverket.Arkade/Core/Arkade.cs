using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Logging;
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
            _testSessionFactory = new TestSessionFactory(new TarCompressionUtility(), new ArchiveIdentifier(),
                new StatusEventHandler());
        }

        public TestSession RunTests(ArchiveFile archive)
        {
            TestSession testSession = _testSessionFactory.NewSessionFromArchive(archive);

            TestEngineFactory f =
                new TestEngineFactory(
                    new TestEngine(new TestProvider(new Noark5TestProvider(new ArchiveContentReader())),
                        new StatusEventHandler()),
                    new AddmlDatasetTestEngine(new FlatFileReaderFactory(), new AddmlProcessRunner()));

            ITestEngine testEngine = f.GetTestEngine(testSession);
            testSession.TestSuite = testEngine.RunTestsOnArchive(testSession);

            return testSession;
        }
    }
}