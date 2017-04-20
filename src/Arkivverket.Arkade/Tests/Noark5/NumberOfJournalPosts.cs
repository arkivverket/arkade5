using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfJournalPosts : Noark5XmlReaderBaseTest
    {
        private readonly int _publicJournalNumberOfJournalPosts;
        private readonly int _runningJournalNumberOfJournalPosts;
        private int _archiveExtractionJournalPostCount;
        private readonly List<TestResult> _testResults = new List<TestResult>();

        public NumberOfJournalPosts(Archive archive)
        {
            try
            {
                _publicJournalNumberOfJournalPosts =
                    GetPostCountFromJournal(ArkadeConstants.PublicJournalXmlFileName, archive);

                _runningJournalNumberOfJournalPosts =
                    GetPostCountFromJournal(ArkadeConstants.RunningJournalXmlFileName, archive);
            }
            catch (Exception)
            {
                _testResults.Add(new TestResult(ResultType.Error, Location.Archive,
                    Noark5Messages.NumberOfJournalPostsMessage_JournalFilesMissing));
            }
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfJournalPosts;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            if (!ActualAndDocumentedNumberOfJournalPostsMatch())
                _testResults.Add(new TestResult(ResultType.Error, Location.Archive,
                    Noark5Messages.NumberOfJournalPostsMessage_ArchiveAndJournalMismatch));

            _testResults.Add(new TestResult(ResultType.Success, Location.Archive,
                string.Format(Noark5Messages.NumberOfJournalPostsMessage_NumberOfJournalPostsFound,
                    _archiveExtractionJournalPostCount)));

            _testResults.Add(new TestResult(ResultType.Success, new Location(ArkadeConstants.PublicJournalXmlFileName),
                string.Format(Noark5Messages.NumberOfJournalPostsMessage_NumberOfJournalPostsInPublicJournal,
                    _publicJournalNumberOfJournalPosts)));

            _testResults.Add(new TestResult(ResultType.Success, new Location(ArkadeConstants.RunningJournalXmlFileName),
                string.Format(Noark5Messages.NumberOfJournalPostsMessage_NumberOfJournalPostsInRunningJournal,
                    _runningJournalNumberOfJournalPosts)));

            return _testResults;
        }

        private bool ActualAndDocumentedNumberOfJournalPostsMatch()
        {
            return _archiveExtractionJournalPostCount == _publicJournalNumberOfJournalPosts &&
                   _archiveExtractionJournalPostCount == _runningJournalNumberOfJournalPosts;
        }

        private static int GetPostCountFromJournal(string journalXmlFileName, Archive archive)
        {
            string journalXmlFile = archive.WorkingDirectory.Content().WithFile(journalXmlFileName).FullName;

            // TODO: Check for file existance to distinguish file not found error from deserialize error

            JournalHead journalHead = JournalGuillotine.Behead(journalXmlFile);

            return journalHead.NumberOfJournalposts;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (IdentifiesJournalPostRegistration(eventArgs))
                _archiveExtractionJournalPostCount++;
        }
        
        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private static bool IdentifiesJournalPostRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("registrering") &&
                   eventArgs.Name.Equals("xsi:type") &&
                   eventArgs.Value.Equals("journalpost");
        }
    }
}
