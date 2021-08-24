using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Base.Siard;
using Serilog;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestEngineFactory
    {
        private readonly ILogger _log = Log.ForContext<TestEngineFactory>();

        private readonly Noark5TestEngine _noark5TestEngine;
        private readonly AddmlDatasetTestEngine _addmlDatasetTestEngine;
        private readonly SiardTestEngine _siardTestEngine;

        public TestEngineFactory(Noark5TestEngine noark5TestEngine, AddmlDatasetTestEngine addmlDatasetTestEngine, SiardTestEngine siardTestEngine)
        {
            _noark5TestEngine = noark5TestEngine;
            _addmlDatasetTestEngine = addmlDatasetTestEngine;
            _siardTestEngine = siardTestEngine;
        }

        public ITestEngine GetTestEngine(TestSession testSession)
        {
            _log.Debug("Find test engine for archive {archiveType}", testSession.Archive.ArchiveType);

            return testSession.Archive.ArchiveType switch
            {
                ArchiveType.Siard => _siardTestEngine,
                ArchiveType.Noark5 => _noark5TestEngine,
                _ => _addmlDatasetTestEngine
            };
        }
    }
}
