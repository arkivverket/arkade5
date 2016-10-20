using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

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

        public AddmlDefinition AddmlDefinition { get; set; }

        private List<LogEntry> LogEntries = new List<LogEntry>();

        public void AddLogEntry(string message)
        {
            LogEntries.Add(new LogEntry(DateTime.Now, message));
        }

        public List<LogEntry> GetLogEntries()
        {
            return LogEntries;
        }

    }
}
