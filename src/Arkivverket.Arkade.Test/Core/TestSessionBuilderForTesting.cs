using Arkivverket.Arkade.Core;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Test.Core
{
    public class TestSessionBuilderForTesting
    {

        private Archive _archive = new Archive("uuid", "workingDir");
        private List<string> _logEntries = new List<string>();

        public TestSessionBuilderForTesting WithArchive(Archive archive)
        {
            _archive = archive;
            return this;
        }

        internal TestSessionBuilderForTesting WithLogEntry(string message)
        {
            _logEntries.Add(message);
            return this;
        }

        public TestSession Build()
        {
            var testSession = new TestSession(_archive);
            foreach (var logEntry in _logEntries)
            {
                testSession.AddLogEntry(logEntry);
            }

            return testSession;
        }
    }
}
