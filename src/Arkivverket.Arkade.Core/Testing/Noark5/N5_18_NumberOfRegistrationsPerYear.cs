using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_18_NumberOfRegistrationsPerYear : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 18);

        private readonly Dictionary<ArchivePart, Dictionary<int, int>> _registrationsByYearPerArchivePart = new();
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
            bool multipleArchiveParts = _registrationsByYearPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();

            foreach ((ArchivePart archivePart, Dictionary<int, int> registrationsByYear) in _registrationsByYearPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((int year, int count) in registrationsByYear.OrderBy(r => r.Key))
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                        string.Format(Noark5Messages.NumberOfXPerY, year, count)));

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
            {
                _currentArchivePart.SystemId = eventArgs.Value;
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
            {
                _currentArchivePart.Name = eventArgs.Value;
            }

            if (eventArgs.Path.Matches("opprettetDato", "registrering"))
            {
                int year;

                if (Noark5TestHelper.TryParseValidXmlDate(eventArgs.Value, out DateTime registrationCreatedTime))
                    year = registrationCreatedTime.Year;
                else
                    return;

                if (_registrationsByYearPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_registrationsByYearPerArchivePart[_currentArchivePart].ContainsKey(year))
                        _registrationsByYearPerArchivePart[_currentArchivePart][year]++;
                    else
                        _registrationsByYearPerArchivePart[_currentArchivePart].Add(year, 1);
                }
                else
                {
                    _registrationsByYearPerArchivePart.Add(_currentArchivePart, new Dictionary<int, int> {{year, 1}});
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
