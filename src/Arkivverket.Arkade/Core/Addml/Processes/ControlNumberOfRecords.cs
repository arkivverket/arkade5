using System.Collections.Generic;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlNumberOfRecords : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 12);

        public const string Name = "Control_NumberOfRecords";
        private readonly List<TestResult> _testResults = new List<TestResult>();
        private FlatFile _currentFlatFile;
        private int _numberOfOcurrencesForCurrentFile;

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlNumberOfRecordsDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override void DoRun(FlatFile flatFile)
        {
            _numberOfOcurrencesForCurrentFile = 0;
            _currentFlatFile = flatFile;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }

        protected override void DoEndOfFile()
        {
            if (!_currentFlatFile.Definition.NumberOfRecords.HasValue)
            {
                _testResults.Add(new TestResult(ResultType.Error, new Location(_currentFlatFile.Definition.FileName),
                    Messages.ControlNumberOfRecordsMessage1));
                return;
            }


            int expectedNumberOfRecords = _currentFlatFile.Definition.NumberOfRecords.Value;
            if (expectedNumberOfRecords > 0)
            {
                if (expectedNumberOfRecords == _numberOfOcurrencesForCurrentFile)
                {
                    _testResults.Add(new TestResult(ResultType.Success,
                        new Location(_currentFlatFile.Definition.FileName),
                        string.Format(Messages.ControlNumberOfRecordsMessage2, expectedNumberOfRecords)));
                }
                else
                {
                    _testResults.Add(new TestResult(ResultType.Error, new Location(_currentFlatFile.Definition.FileName),
                        string.Format(Messages.ControlNumberOfRecordsMessage3, expectedNumberOfRecords,
                            _numberOfOcurrencesForCurrentFile)));
                }
            }
        }

        protected override void DoRun(Record record)
        {
            _numberOfOcurrencesForCurrentFile++;
        }

        protected override void DoRun(Field field)
        {
        }
    }
}