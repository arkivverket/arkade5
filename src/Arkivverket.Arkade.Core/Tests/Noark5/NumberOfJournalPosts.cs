using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Tests.Noark5
{
    public class NumberOfJournalPosts : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 59);

        private readonly int _publicJournalNumberOfJournalPosts;
        private readonly int _runningJournalNumberOfJournalPosts;
        private int _archiveExtractionJournalPostCount;
        private readonly List<TestResult> _testResults = new List<TestResult>();
        private readonly bool _periodSeparationIsSharp;

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
                    Noark5Messages.NumberOfJournalPostsMessage_CouldNotReadFromJournals));
            }

            _periodSeparationIsSharp = Noark5TestHelper.PeriodSeparationIsSharp(archive);
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfJournalPosts;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            if (!PublicAndRunningJournalNumbersMatch())
                _testResults.Add(new TestResult(ResultType.Error, Location.Archive,
                    Noark5Messages.NumberOfJournalPostsMessage_UnEqualJournalNumbers));

            if (_periodSeparationIsSharp && !NumberOfJournalPostsInArchiveAndJournalsMatch())
                _testResults.Add(new TestResult(ResultType.Error, Location.Archive,
                    Noark5Messages.NumberOfJournalPostsMessage_UnEqualJournalAndArchiveNumbers));

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

        private bool NumberOfJournalPostsInArchiveAndJournalsMatch()
        {
            return _archiveExtractionJournalPostCount == _publicJournalNumberOfJournalPosts &&
                   _archiveExtractionJournalPostCount == _runningJournalNumberOfJournalPosts;
        }

        private bool PublicAndRunningJournalNumbersMatch()
        {
            return _publicJournalNumberOfJournalPosts == _runningJournalNumberOfJournalPosts;
        }

        private static int GetPostCountFromJournal(string journalXmlFileName, Archive archive)
        {
            string journalXmlFile = archive.WorkingDirectory.Content().WithFile(journalXmlFileName).FullName;

            JournalHead journalHead = JournalGuillotine.Behead(journalXmlFile);

            return journalHead.NumberOfJournalposts;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesJournalPostRegistration(eventArgs))
                _archiveExtractionJournalPostCount++;
        }
        
        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
