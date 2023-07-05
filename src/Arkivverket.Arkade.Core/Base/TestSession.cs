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

        public bool TestRunContainsDocumentFileDependentTests =>
            TestsToRun.Any(test => DocumentFileDependentNoark5Tests.Contains(test));

        public bool TestRunContainsChecksumControl =>
            TestsToRun.Contains(TestId.Create("N5.30"));

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

        public bool IsTestableArchive(out string disqualifyingCause)
        {
            disqualifyingCause = "";

            switch (Archive.ArchiveType)
            {
                case ArchiveType.Siard:
                    FileInfo[] fileInfos = Archive.WorkingDirectory.Content().DirectoryInfo().GetFiles("*.siard");
                    if (fileInfos.FirstOrDefault() == default)
                        disqualifyingCause = Resources.SiardMessages.CouldNotFindASiardFile;
                    else if (Archive.Details == null)
                        disqualifyingCause = Resources.SiardMessages.ValidatorDoesNotSupportVersionMessage;
                    else
                        return true;
                    return false;

                case ArchiveType.Noark5:
                    if (!Archive.AddmlXmlUnit.File.Exists)
                    {
                        disqualifyingCause = Resources.Noark5Messages.CouldNotFindValidSpecificationFile;
                        return false;
                    }
                    break;

                case ArchiveType.Noark4:
                    disqualifyingCause = Resources.Messages.Noark4ValidationNotSupported;
                    return false;

                default:
                    if (AddmlDefinition == null)
                    {
                        disqualifyingCause = Resources.Noark5Messages.CouldNotFindValidSpecificationFile;
                        return false;
                    }
                    break;
            }

            return true;
        }
    }
}