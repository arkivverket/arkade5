using Arkivverket.Arkade.Core.Addml;
using Serilog;

namespace Arkivverket.Arkade.Core
{
    public class TestEngineFactory
    {
        private readonly ILogger _log = Log.ForContext<TestEngineFactory>();

        private readonly TestEngine _noark5TestEngine;
        private readonly AddmlDatasetTestEngine _addmlDatasetTestEngine;

        public TestEngineFactory(TestEngine noark5TestEngine, AddmlDatasetTestEngine addmlDatasetTestEngine)
        {
            _noark5TestEngine = noark5TestEngine;
            _addmlDatasetTestEngine = addmlDatasetTestEngine;
        }

        public ITestEngine GetTestEngine(TestSession testSession)
        {
            _log.Debug("Find test engine for archive {archiveType}", testSession.Archive.ArchiveType);

            if (testSession.Archive.ArchiveType == ArchiveType.Noark5)
            {
                return _noark5TestEngine;
            }
            else
            {
                return _addmlDatasetTestEngine;
            }
        }
    }
}
