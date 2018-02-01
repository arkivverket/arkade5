using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core
{
    public class TestSession
    {
        private readonly List<LogEntry> LogEntries = new List<LogEntry>();
        private static readonly ILogger Log = Serilog.Log.ForContext<TestSession>();

        public Archive Archive { get; }

        public ArchiveMetadata ArchiveMetadata { get; set; }

        public TestSuite TestSuite { get; set; }

        public TestSummary TestSummary { get; set; }

        public AddmlDefinition AddmlDefinition { get; set; }

        public DateTime DateOfTesting { get; }

        private readonly bool _archiveHasOversizedFiles;

        public TestSession(Archive archive)
        {
            Archive = archive;
            DateOfTesting = DateTime.Now;
            _archiveHasOversizedFiles = ArchiveHasOversizedFiles();
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

        public bool IsPackableArchive()
        {
            return !_archiveHasOversizedFiles;
        }

        private bool ArchiveHasOversizedFiles()
        {
            IEnumerable<FileInfo> contentDirectoryFiles =
                Archive.WorkingDirectory.Content().DirectoryInfo().EnumerateFiles(".", SearchOption.AllDirectories);

            const long singleFileSizeLimit = 8589934592; // 8 GB

            IEnumerable<FileInfo> overSizedFiles =
                contentDirectoryFiles.Where(file => file.Length > singleFileSizeLimit).ToArray();

            const string message =
                "Un-packable file found: {0}. The file size {1} B. is more than the current single file size limit of 8 GB.";

            foreach (FileInfo file in overSizedFiles)
                Log.Warning(string.Format(message, file.Name, file.Length));

            return overSizedFiles.Any();
        }
    }
}