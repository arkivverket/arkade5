using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_35_NumberOfCaseParts : Noark5XmlReaderBaseTest
    {
        private ArchivePart _currentArchivePart = new();
        private int _totalNumberOfCaseParts;
        private Archive archive;
        private readonly Dictionary<ArchivePart, int> _casePartsPerArchivePart = new();
        private readonly TestId _id;

        private string GetTestVersion()
        {
            return archive.Details.ArchiveStandard;
        }

        public N5_35_NumberOfCaseParts(Archive archive)
        {
            this.archive = archive;
            _id = new TestId(TestId.TestKind.Noark5, 35, GetTestVersion());
        }

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
            bool multipleArchiveParts = _casePartsPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _totalNumberOfCaseParts))
                }
            };

            if (_totalNumberOfCaseParts == 0 || !multipleArchiveParts)
                return testResultSet;

            foreach ((ArchivePart archivePart, int casePartCount) in _casePartsPerArchivePart)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfXPerY, archivePart, casePartCount)));

            return testResultSet;
        }


        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                _casePartsPerArchivePart.Add(_currentArchivePart, 0);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (archive.Details.ArchiveStandard.Equals("5.0"))
            {
                CountPartsForVersion5_5(eventArgs);
            }

            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();

            CountCaseParts(eventArgs);
        }

        private void CountPartsForVersion5_5(ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("part"))
            {
                Count();
            }
        }

        private void CountCaseParts(ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("sakspart"))
            {
                Count();
            }
        }

        private void Count()
        {
            _totalNumberOfCaseParts++;

            if (_casePartsPerArchivePart.Count > 0)
            {
                if (_casePartsPerArchivePart.ContainsKey(_currentArchivePart))
                    _casePartsPerArchivePart[_currentArchivePart]++;
            }
        }
    }
}