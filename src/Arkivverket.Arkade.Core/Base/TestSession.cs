using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestSession
    {
        private readonly List<LogEntry> LogEntries = new List<LogEntry>();

        public Archive Archive { get; }

        public ArchiveMetadata ArchiveMetadata { get; set; }

        public List<TestId> TestsToRun { get; set; } = new List<TestId>();
        public List<TestId> AvailableTests { get; set; } = new List<TestId>();

        public TestSuite TestSuite { get; set; }

        public TestSummary TestSummary { get; set; }

        public AddmlDefinition AddmlDefinition { get; set; }

        public DateTime DateOfTesting { get; }

        public bool GenerateFileFormatInfo { get; set; }

        public SupportedLanguage OutputLanguage { get; set; }

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

        public bool IsTestableArchive()
        {
            return Archive.ArchiveType != ArchiveType.Siard &&
                   AddmlDefinition != null ||
                   Archive.ArchiveType == ArchiveType.Noark5 && Archive.AddmlXmlUnit.File.Exists;
        }
    }
}