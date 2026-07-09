using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestSession
    {
        private readonly List<LogEntry> LogEntries = new List<LogEntry>();

        private static readonly List<TestId> DocumentFileDependentNoark5Tests = new()
        {
            TestId.Create("N5.28"),
            TestId.Create("N5.30"),
            TestId.Create("N5.32"),
            TestId.Create("N5.33"),
            TestId.Create("N5.64")
        };

        public List<TestId> TestsToRun { get; set; } = new List<TestId>();

        public TestSuite TestSuite { get; set; }

        public TestSummary TestSummary { get; set; }

        public AddmlDefinition AddmlDefinition { get; set; }

        public DateTime TimeOfTesting { get; set; }
        
        public SupportedLanguage OutputLanguage { get; set; }

        public DirectoryInfo TemporaryTestResultFilesDirectory { get; set; }


        public bool TestRunContainsDocumentFileDependentTests =>
            TestsToRun.Any(test => DocumentFileDependentNoark5Tests.Contains(test));

        public bool TestRunContainsChecksumControl =>
            TestsToRun.Contains(TestId.Create("N5.30"));

        public TestSession(DirectoryInfo temporaryTestResultFilesDirectory)
        {
            TemporaryTestResultFilesDirectory = temporaryTestResultFilesDirectory;
        }

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