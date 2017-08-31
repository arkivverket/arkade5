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

        public ArchiveMetadata ArchiveMetadata { get; set; }

        public TestSuite TestSuite { get; set; }

        public TestSummary TestSummary { get; set; }

        public AddmlDefinition AddmlDefinition { get; set; }

        public DateTime DateOfTesting { get; }

        public TestSession(Archive archive)
        {
            Archive = archive;
            DateOfTesting = DateTime.Now;
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
            return Archive.WorkingDirectory.RepositoryOperations().DirectoryInfo();
        }

        public bool IsTestableArchive()
        {
            return AddmlDefinition != null || Archive.ArchiveType == ArchiveType.Noark5;
        }
    }
}