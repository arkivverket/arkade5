using Arkivverket.Arkade.Core;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Test.Core
{
    public class TestSessionBuilderForTesting
    {

        private ArchiveBuilder _archiveBuilder = new ArchiveBuilder();
        private List<string> _logEntries = new List<string>();

        internal TestSessionBuilderForTesting WithLogEntry(string message)
        {
            _logEntries.Add(message);
            return this;
        }

        public TestSession Build()
        {
            Archive archive = _archiveBuilder.Build();
            var testSession = new TestSession(archive);
            foreach (var logEntry in _logEntries)
            {
                testSession.AddLogEntry(logEntry);
            }

            return testSession;
        }
    }
}
