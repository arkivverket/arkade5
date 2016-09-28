using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core
{

    public class TestSession
    {

        public TestSession() { }

        public Archive Archive { get; }

        public ArchiveInfo ArchiveInfo { get; }

        public TestSuite TestRun { get; }

        public TestReport TestReport { get; }

        private List<LogEntry> logEntries = new List<LogEntry>();

        public void AddLogEntry(string message)
        {
            logEntries.Add(new LogEntry(new DateTime(), message));
        }

    }
}
