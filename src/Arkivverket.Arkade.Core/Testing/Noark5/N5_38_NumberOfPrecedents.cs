using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
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
    public class N5_38_NumberOfPrecedents : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 38);

        private bool _journalPostAttributeIsFound;
        private bool _caseFolderAttributeIsFound;
        private N5_38_ArchivePart _currentArchivePart;
        private readonly List<N5_38_ArchivePart> _archiveParts = new();


        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override TestResultSet GetTestResults()
        {
            bool multipleArchiveParts = _archiveParts.Count > 1;

            int totalNumberOfPrecedents = _archiveParts.Sum(CountTotalNumberOfPrecedents);

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfPrecedents))
                }
            };

            if (totalNumberOfPrecedents == 0)
                return testResultSet;

            foreach (N5_38_ArchivePart archivePart in _archiveParts)
            {
                var testResults = new List<TestResult>();

                if (archivePart.NumberOfPrecedentsInJournalposts > 0)
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfPrecedentsInJournalpostsMessage,
                        archivePart.NumberOfPrecedentsInJournalposts)));

                if (archivePart.NumberOfPrecedentsInCasefolders > 0)
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfPrecedentsInCaseFolderMessage,
                        archivePart.NumberOfPrecedentsInCasefolders)));

                if(multipleArchiveParts)
                {
                    testResults.Insert(0, new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOf, CountTotalNumberOfPrecedents(archivePart))));

                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                }
                else
                    testResultSet.TestsResults.AddRange(testResults);
            }

            return testResultSet;
        }

        private int CountTotalNumberOfPrecedents(N5_38_ArchivePart currentArchivePart)
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
                if (_caseFolderAttributeIsFound)
                    _currentArchivePart.NumberOfPrecedentsInCasefolders++;
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesJournalPostRegistration(eventArgs))
                _journalPostAttributeIsFound = true;

            if (Noark5TestHelper.IdentifiesCasefolder(eventArgs))
                _caseFolderAttributeIsFound = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart = new N5_38_ArchivePart { SystemId = eventArgs.Value };
                _archiveParts.Add(_currentArchivePart);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            // TODO: Handle non-journalpost-type subregistrations?
            if (eventArgs.NameEquals("registrering"))
                _journalPostAttributeIsFound = false;

            // TODO: Handle non-casefolder-type subfolders?
            if (eventArgs.NameEquals("mappe"))
                _caseFolderAttributeIsFound = false;
        }

        private class N5_38_ArchivePart : ArchivePart
        {
            public int NumberOfPrecedentsInCasefolders { get; set; }
            public int NumberOfPrecedentsInJournalposts { get; set; }
        }
    }
}
