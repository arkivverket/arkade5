using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_11_NumberOfFoldersPerYear : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 11);

        private Dictionary<ArchivePart, Dictionary<int, int>> _foldersByYearPerArchivePart = new();
        private ArchivePart _currentArchivePart = new();

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
            var testResultSet = new TestResultSet();
            bool multipleArchiveParts = _foldersByYearPerArchivePart.Count > 1;

            foreach ((ArchivePart archivePart, Dictionary<int, int> foldersByYear) in _foldersByYearPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((int year, int count) in foldersByYear.ToImmutableSortedDictionary())
                    testResults.Add(new TestResult(ResultType.Success, new Location(""),
                        string.Format(Noark5Messages.NumberOfFoldersPerYear, year, count)));

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                else
                    testResultSet.TestsResults = testResults;
            }

            return testResultSet;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("opprettetDato", "mappe"))
            {
                int year;

                if (Noark5TestHelper.TryParseValidXmlDate(eventArgs.Value, out DateTime registrationCreatedTime))
                    year = registrationCreatedTime.Year;
                else
                    return;

                if (_foldersByYearPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_foldersByYearPerArchivePart[_currentArchivePart].ContainsKey(year))
                        _foldersByYearPerArchivePart[_currentArchivePart][year]++;
                    else
                        _foldersByYearPerArchivePart[_currentArchivePart].Add(year, 1);
                }
                else
                {
                    _foldersByYearPerArchivePart.Add(_currentArchivePart, new Dictionary<int, int> {{year, 1}});
                }
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
                _currentArchivePart = new ArchivePart();
            }
        }
    }
}
