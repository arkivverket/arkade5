using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_11_NumberOfFoldersPerYear : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 11);

        private N5_11_ArchivePart _currentArchivePart = new N5_11_ArchivePart();
        private readonly List<N5_11_ArchivePart> _archiveParts = new List<N5_11_ArchivePart>();

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

            foreach (N5_11_ArchivePart archivePart in _archiveParts)
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
                            string.Format(Noark5Messages.NumberOfFoldersPerYear_ForArchivePart, archivePart.SystemId, archivePart.Name,
                                year, count)));
                    }
                }
            }

            return testResults;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

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
            if (eventArgs.NameEquals("arkivdel"))
            {
                _archiveParts.Add(_currentArchivePart);
                _currentArchivePart = new N5_11_ArchivePart();
            }
        }

        private class N5_11_ArchivePart : ArchivePart
        {
            public readonly Dictionary<int, int> FoldersByYear = new Dictionary<int, int>();
        }
    }
}
