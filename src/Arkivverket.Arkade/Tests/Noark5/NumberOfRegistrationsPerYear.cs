using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #14
    /// </summary>
    public class NumberOfRegistrationsPerYear : Noark5XmlReaderBaseTest
    {
        private readonly Dictionary<int, int> _registrationsByYear = new Dictionary<int, int>();

        public override string GetName()
        {
            return Noark5Messages.NumberOfRegistrationsPerYear;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            var registrationsByYearOrdered = _registrationsByYear.OrderBy(r => r.Key);

            foreach (KeyValuePair<int, int> registrationsAtYear in registrationsByYearOrdered)
            {
                int year = registrationsAtYear.Key;
                int count = registrationsAtYear.Value;

                testResults.Add(new TestResult(ResultType.Success, new Location(""), year + ": " + count));
            }

            return testResults;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (!eventArgs.Path.Matches("opprettetDato", "registrering"))
                return;

            int year = DateTime.Parse(eventArgs.Value).Year;

            if (_registrationsByYear.ContainsKey(year))
                _registrationsByYear[year]++;
            else
                _registrationsByYear.Add(year, 1);
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
