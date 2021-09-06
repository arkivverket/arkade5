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