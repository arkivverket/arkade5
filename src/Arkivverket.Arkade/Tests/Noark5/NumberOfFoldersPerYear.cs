using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfFoldersPerYear : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 11);

        private ArchivePart _currentArchivePart;
        private readonly List<ArchivePart> _archiveParts = new List<ArchivePart>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfFoldersPerYear;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            foreach (ArchivePart archivePart in _archiveParts)
            {
                var foldersByYearOrdered = archivePart.FoldersByYear.OrderBy(r => r.Key);

                foreach (KeyValuePair<int, int> foldersAtYear in foldersByYearOrdered)
                {
                    int year = foldersAtYear.Key;
                    int count = foldersAtYear.Value;

                    if (_archiveParts.Count == 1)
                        testResults.Add(new TestResult(ResultType.Success, new Location(""), year + ": " + count));
                    else
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfFoldersPerYear_ForArchivePart, archivePart.SystemId,
                                year, count)));
                    }
                }
            }

            return testResults;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart = new ArchivePart {SystemId = eventArgs.Value};
                _archiveParts.Add(_currentArchivePart);
            }

            if (eventArgs.Path.Matches("opprettetDato", "mappe"))
            {
                int year = DateTime.Parse(eventArgs.Value).Year;

                if (_currentArchivePart.FoldersByYear.ContainsKey(year))
                    _currentArchivePart.FoldersByYear[year]++;
                else
                    _currentArchivePart.FoldersByYear.Add(year, 1);
            }
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class ArchivePart
        {
            public readonly Dictionary<int, int> FoldersByYear = new Dictionary<int, int>();
            public string SystemId { get; set; }
        }
    }
}
