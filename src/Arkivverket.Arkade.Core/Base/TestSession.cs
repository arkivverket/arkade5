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

        public Uuid InputDiasPackageId { get; }

        public List<TestId> TestsToRun { get; set; } = new List<TestId>();

        public TestSuite TestSuite { get; set; }

        public TestSummary TestSummary { get; set; }

        public AddmlDefinition AddmlDefinition { get; set; }

        public DateTime DateOfTesting { get; }
        
        public SupportedLanguage OutputLanguage { get; set; }
        

        public bool TestRunContainsDocumentFileDependentTests =>
            TestsToRun.Any(test => DocumentFileDependentNoark5Tests.Contains(test));

        public bool TestRunContainsChecksumControl =>
            TestsToRun.Contains(TestId.Create("N5.30"));

        public TestSession(Archive archive, Uuid inputDiasPackageId = null)
        {
            Archive = archive;
            InputDiasPackageId = inputDiasPackageId;
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

        public static bool IsTestableArchive(Archive archive, AddmlDefinition addmlDefinition, out string disqualifyingCause)
        {
            disqualifyingCause = "";

            switch (archive.ArchiveType)
            {
                case ArchiveType.Siard:
                    FileInfo[] fileInfos = archive.WorkingDirectory.Content().DirectoryInfo().GetFiles("*.siard");
                    if (fileInfos.FirstOrDefault() == default)
                        disqualifyingCause = Resources.SiardMessages.CouldNotFindASiardFile;
                    else if (archive.Details == null)
                        disqualifyingCause = Resources.SiardMessages.ValidatorDoesNotSupportVersionMessage;
                    else
                        return true;
                    return false;

                case ArchiveType.Noark5:
                    if (!archive.AddmlXmlUnit.File.Exists)
                    {
                        disqualifyingCause = Resources.Noark5Messages.CouldNotFindValidSpecificationFile;
                        return false;
                    }
                    break;

                case ArchiveType.Noark4:
                    disqualifyingCause = Resources.Messages.Noark4ValidationNotSupported;
                    return false;

                default:
                    if (addmlDefinition == null)
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