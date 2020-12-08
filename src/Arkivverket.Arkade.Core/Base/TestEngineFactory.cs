using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Noark5;
using Serilog;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestEngineFactory
    {
        private readonly ILogger _log = Log.ForContext<TestEngineFactory>();

        private readonly Noark5TestEngine _noark5TestEngine;
        private readonly AddmlDatasetTestEngine _addmlDatasetTestEngine;

        public TestEngineFactory(Noark5TestEngine noark5TestEngine, AddmlDatasetTestEngine addmlDatasetTestEngine)
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
