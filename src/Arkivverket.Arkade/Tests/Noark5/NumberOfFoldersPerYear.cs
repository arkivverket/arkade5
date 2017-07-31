using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #08
    /// </summary>
    public class NumberOfFoldersPerYear : Noark5XmlReaderBaseTest
    {
        private readonly Dictionary<int, int> _foldersByYear = new Dictionary<int, int>();

        public override string GetName()
        {
            return Noark5Messages.NumberOfFoldersPerYear;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            var foldersByYearOrdered = _foldersByYear.OrderBy(r => r.Key);

            foreach (KeyValuePair<int, int> foldersAtYear in foldersByYearOrdered)
            {
                int year = foldersAtYear.Key;
                int count = foldersAtYear.Value;

                testResults.Add(new TestResult(ResultType.Success, new Location(""), year + ": " + count));
            }

            return testResults;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (!eventArgs.Path.Matches("opprettetDato", "mappe"))
                return;

            int year = DateTime.Parse(eventArgs.Value).Year;

            if (_foldersByYear.ContainsKey(year))
                _foldersByYear[year]++;
            else
                _foldersByYear.Add(year, 1);
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
    }
}
