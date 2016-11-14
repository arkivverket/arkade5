using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlNumberOfRecords : IAddmlProcess
    {
        private const string Name = "Control_NumberOfRecords";
        private readonly TestRun _testRun;
        private FlatFile _currentFlatFile;
        private int _numberOfOcurrencesForCurrentFile;

        public ControlNumberOfRecords()
        {
            _testRun = new TestRun(GetType().FullName, TestType.Content);
        }

        public string GetName()
        {
            return Name;
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
                _testRun.Add(new TestResult(ResultType.Error, new Location(_currentFlatFile.Definition.FileName), "Expected number of records not specified. Unable to control number of records."));
                return;
            }


            int expectedNumberOfRecords = _currentFlatFile.Definition.NumberOfRecords.Value;
            if (expectedNumberOfRecords > 0)
            {
                if (expectedNumberOfRecords == _numberOfOcurrencesForCurrentFile)
                {
                    _testRun.Add(new TestResult(ResultType.Success, new Location(_currentFlatFile.Definition.FileName),
                        $"Number of records ({expectedNumberOfRecords}) matched for file {_currentFlatFile.Definition.FileName}."));
                }
                else
                {
                    _testRun.Add(new TestResult(ResultType.Error, new Location(_currentFlatFile.Definition.FileName),
                        $"Number of records did not match for file {_currentFlatFile.Definition.FileName}. Expected {expectedNumberOfRecords}, found {_numberOfOcurrencesForCurrentFile}. "));
                }
            }
        }

        public void Run(Record record)
        {
            _numberOfOcurrencesForCurrentFile++;
        }
    }
}