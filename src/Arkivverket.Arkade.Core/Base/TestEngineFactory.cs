using System;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Archives;
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

        public ITestEngine GetTestEngine(Archive archive)
        {
            _log.Debug("Find test engine for {archiveTypeName}", archive.GetType().Name);

            return archive switch
            {
                SiardArchive => _siardTestEngine,
                Noark5Archive => _noark5TestEngine,
                AddmlDefinitionTestedArchive => _addmlDatasetTestEngine,
                _ => throw new ArgumentOutOfRangeException(nameof(archive))
            };
        }
    }
}
