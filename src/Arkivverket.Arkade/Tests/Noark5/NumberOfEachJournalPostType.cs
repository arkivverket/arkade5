using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfEachJournalPostType : Noark5XmlReaderBaseTest
    {
        private readonly List<JournalPost> _journalPosts = new List<JournalPost>();
        private readonly List<TestResult> _testResults = new List<TestResult>();
        private string _currentArchivePartSystemId;
        private string _currentJournalPostSystemId;
        private bool _journalPostAttributeIsFound;
        private bool _mainDocumentBindingIsFound;

        public override string GetName()
        {
            return Noark5Messages.NumberOfEachJournalPostType;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var journalPostQuery = from journalPost in _journalPosts
                group journalPost by new
                {
                    journalPost.ArchivePartSystemId,
                    journalPost.JournalpostType
                }
                into grouped
                select new
                {
                    grouped.Key.ArchivePartSystemId,
                    grouped.Key.JournalpostType,
                    Count = grouped.Count()
                };


            bool multipleArchiveParts = _journalPosts.GroupBy(j => j.ArchivePartSystemId).Count() > 1;

            foreach (var item in journalPostQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfEachJournalPostTypeMessage_TypeAndCount,
                        item.JournalpostType, item.Count
                    )
                );

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, item.ArchivePartSystemId) + " - ");

                _testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            return _testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (IdentifiesJournalPostRegistration(eventArgs))
                _journalPostAttributeIsFound = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("systemID", "registrering") && _journalPostAttributeIsFound)
                _currentJournalPostSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("journalposttype", "registrering") && _journalPostAttributeIsFound)
                _journalPosts.Add(new JournalPost(eventArgs.Value, _currentArchivePartSystemId));

            if (eventArgs.Path.Matches("tilknyttetRegistreringSom") && eventArgs.Value.Equals("Hoveddokument"))
                _mainDocumentBindingIsFound = true;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (!eventArgs.NameEquals("registrering"))
                return;

            if (!_mainDocumentBindingIsFound)
                _testResults.Add(new TestResult(ResultType.Error, new Location(""),
                    string.Format(Noark5Messages.NumberOfEachJournalPostTypeMessage_MissingMainDocument,
                        _currentJournalPostSystemId)));

            _journalPostAttributeIsFound = false; // reset
            _currentJournalPostSystemId = ""; // reset
            _mainDocumentBindingIsFound = false; // reset
        }

        private static bool IdentifiesJournalPostRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.Path.Matches("registrering") &&
                   eventArgs.Name.Equals("xsi:type") &&
                   eventArgs.Value.Equals("journalpost");
        }

        internal class JournalPost
        {
            public string ArchivePartSystemId { get; }
            public string JournalpostType { get; }

            public JournalPost(string journalpostType, string archivePartSystemId)
            {
                JournalpostType = journalpostType;
                ArchivePartSystemId = archivePartSystemId;
            }
        }
    }
}
