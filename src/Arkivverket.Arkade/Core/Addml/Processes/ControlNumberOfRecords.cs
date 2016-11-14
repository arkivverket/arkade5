using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlNumberOfRecords : IAddmlProcess
    {
        public const string Name = "Control_NumberOfRecords";

        private readonly TestRun _testRun;
        private FlatFile _currentFlatFile;
        private int _numberOfOcurrencesForCurrentFile;

        public ControlNumberOfRecords()
        {
            _testRun = new TestRun(Name, TestType.Content);
        }

        public string GetName()
        {
            return Name;
        }

        public string GetDescription()
        {
            return Messages.ControlNumberOfRecordsDescription;
        }

        public void Run(FlatFile flatFile)
        {
            _numberOfOcurrencesForCurrentFile = 0;
            _currentFlatFile = flatFile;
        }

        public TestRun GetTestRun()
        {
            return _testRun;
        }

        public void Run(Field field)
        {
        }

        public void EndOfFile()
        {
            if (!_currentFlatFile.Definition.NumberOfRecords.HasValue)
            {
                _testRun.Add(new TestResult(ResultType.Error, new Location(_currentFlatFile.Definition.FileName),
                    Messages.ControlNumberOfRecordsMessage1));
                return;
            }


            int expectedNumberOfRecords = _currentFlatFile.Definition.NumberOfRecords.Value;
            if (expectedNumberOfRecords > 0)
            {
                if (expectedNumberOfRecords == _numberOfOcurrencesForCurrentFile)
                {
                    _testRun.Add(new TestResult(ResultType.Success, new Location(_currentFlatFile.Definition.FileName),
                        string.Format(Messages.ControlNumberOfRecordsMessage2, expectedNumberOfRecords)));
                }
                else
                {
                    _testRun.Add(new TestResult(ResultType.Error, new Location(_currentFlatFile.Definition.FileName),
                        string.Format(Messages.ControlNumberOfRecordsMessage3, expectedNumberOfRecords,
                            _numberOfOcurrencesForCurrentFile)));
                }
            }
        }

        public void Run(Record record)
        {
            _numberOfOcurrencesForCurrentFile++;
        }
    }
}