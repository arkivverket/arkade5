using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core
{

    public class TestSession
    {

        public TestSession(Archive archive)
        {
            Archive = archive;
        }

        public Archive Archive { get; }

        public ArchiveInfo ArchiveInfo { get; }

        public TestSuite TestSuite { get; set; }

        public TestReport TestReport { get; }

        private List<LogEntry> LogEntries = new List<LogEntry>();

        public void AddLogEntry(string message)
        {
            LogEntries.Add(new LogEntry(new DateTime(), message));
        }

        public List<LogEntry> GetLogEntries()
        {
            return LogEntries;
        }

    }
}
