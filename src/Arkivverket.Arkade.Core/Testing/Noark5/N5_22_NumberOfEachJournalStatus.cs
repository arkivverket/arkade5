using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_22_NumberOfEachJournalStatus : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 22);

        private string _currentArchivePartSystemId;
        private JournalPost _currentJournalPost;
        private readonly List<JournalPost> _journalPosts = new List<JournalPost>();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            var journalPostQuery = from journalPost in _journalPosts
                group journalPost by new
                {
                    journalPost.ArchivePartSystemId,
                    journalPost.Status
                }
                into grouped
                select new
                {
                    grouped.Key.ArchivePartSystemId,
                    grouped.Key.Status,
                    Count = grouped.Count()
                };

            bool multipleArchiveParts = _journalPosts.GroupBy(j => j.ArchivePartSystemId).Count() > 1;

            foreach (var item in journalPostQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfEachJournalStatusMessage, item.Status, item.Count));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, item.ArchivePartSystemId) + " - ");

                ResultType resultType = item.Status.Equals("Arkivert") || item.Status.Equals("Utgår")
                    ? ResultType.Success
                    : ResultType.Error;

                testResults.Add(new TestResult(resultType, new Location(""), message.ToString()));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesJournalPostRegistration(eventArgs))
                _currentJournalPost = new JournalPost {ArchivePartSystemId = _currentArchivePartSystemId};
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("journalstatus", "registrering") && _currentJournalPost != null)
                _currentJournalPost.Status = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering") && _currentJournalPost != null)
            {
                _journalPosts.Add(_currentJournalPost);
                _currentJournalPost = null;
            }
        }

        private class JournalPost
        {
            public string ArchivePartSystemId { get; set; }
            public string Status { get; set; }
        }
    }
}
