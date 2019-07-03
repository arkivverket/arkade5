using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    /// <inheritdoc />
    /// <summary>
    ///  Antall presedenser i saksmapper: NN
    ///  Antall presedenser i journalposter: NN
    /// </summary>
    public class NumberOfPrecedents : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 38);

        private bool _journalPostAttributeIsFound;
        private bool _casefolderAttributeIsFound;
        private ArchivePart _currentArchivePart;
        private readonly List<ArchivePart> _archiveParts = new List<ArchivePart>();


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
            int totalNumberOfPrecedents = 0;


            if (_archiveParts.Count == 1)
            {
                testResults.Add(new TestResult(ResultType.Success,
                    new Location(""),
                    string.Format(Noark5Messages.NumberOfPrecedentsInCaseFolderMessage,
                        _currentArchivePart.NumberOfPrecedentsInCasefolders
                    )));
                testResults.Add(new TestResult(ResultType.Success,
                    new Location(""),
                    string.Format(Noark5Messages.NumberOfPrecedentsInJournalpostsMessage,
                        _currentArchivePart.NumberOfPrecedentsInJournalposts
                    )));

                totalNumberOfPrecedents = CountTotalNumberOfPrecedents(_currentArchivePart);
            }
            else
            {
                foreach (ArchivePart archivePart in _archiveParts)
                {
                    if (archivePart.NumberOfPrecedentsInJournalposts > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfPrecedentsInJournalpostsMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.NumberOfPrecedentsInJournalposts)));
                    }

                    if (archivePart.NumberOfPrecedentsInCasefolders > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfPrecedentsInCaseFolderMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.NumberOfPrecedentsInCasefolders)));
                    }

                    totalNumberOfPrecedents += CountTotalNumberOfPrecedents(archivePart);
                }
            }

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""), 
                string.Format(Noark5Messages.TotalResultNumber, totalNumberOfPrecedents.ToString())));

            return testResults;
        }

        private int CountTotalNumberOfPrecedents(ArchivePart currentArchivePart)
        {
            int totalNumberOfPrecedentsResult = new int[]
            {
                currentArchivePart.NumberOfPrecedentsInCasefolders,
                currentArchivePart.NumberOfPrecedentsInJournalposts
            }.Sum();

            return totalNumberOfPrecedentsResult;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("presedens", "registrering"))
            {
                if (_journalPostAttributeIsFound)
                    _currentArchivePart.NumberOfPrecedentsInJournalposts++;
            }

            if (eventArgs.Path.Matches("presedens", "mappe"))
            {
                if (_casefolderAttributeIsFound)
                    _currentArchivePart.NumberOfPrecedentsInCasefolders++;
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesJournalPostRegistration(eventArgs))
                _journalPostAttributeIsFound = true;

            if (Noark5TestHelper.IdentifiesCasefolder(eventArgs))
                _casefolderAttributeIsFound = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart = new ArchivePart {SystemId = eventArgs.Value};
                _archiveParts.Add(_currentArchivePart);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            // TODO: Handle non-journalpost-type subregistrations?
            if (eventArgs.NameEquals("registrering"))
                _journalPostAttributeIsFound = false;

            // TODO: Handle non-casefolder-type subfolders?
            if (eventArgs.NameEquals("mappe"))
                _casefolderAttributeIsFound = false;
        }

        private class ArchivePart
        {
            public string SystemId { get; set; }
            public int NumberOfPrecedentsInCasefolders { get; set; }
            public int NumberOfPrecedentsInJournalposts { get; set; }
        }
    }
}
