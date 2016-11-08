using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core
{
    public class TestSession
    {
        private readonly List<LogEntry> LogEntries = new List<LogEntry>();

        public Archive Archive { get; }

        public ArchiveInfo ArchiveInfo { get; }

        public TestSuite TestSuite { get; set; }

        public TestReport TestReport { get; }

        public AddmlDefinition AddmlDefinition { get; set; }

        public TestSession(Archive archive)
        {
            Archive = archive;
        }

        public void AddLogEntry(string message)
        {
            LogEntries.Add(new LogEntry(DateTime.Now, message));
        }

        public List<LogEntry> GetLogEntries()
        {
            return LogEntries;
        }

        public DirectoryInfo GetReportDirectory()
        {
            DirectoryInfo workDir = Archive.WorkingDirectory;
            DirectoryInfo reportDir = new DirectoryInfo(Path.Combine(workDir.FullName, "reports"));

            if (!reportDir.Exists)
            {
                reportDir.Create();
            }

            return reportDir;
        }
    }
}