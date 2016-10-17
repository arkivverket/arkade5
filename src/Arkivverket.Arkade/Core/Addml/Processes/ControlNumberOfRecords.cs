using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlNumberOfRecords : IAddmlProcess, IAddmlFileProcess, IAddmlRecordProcess
    {
        private readonly TestRun _testRun;
        private FlatFile _currentFlatFile;
        private int _numberOfOcurrencesForCurrentFile;

        public ControlNumberOfRecords()
        {
            _testRun = new TestRun(GetType().FullName, TestType.Content);
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

        public void EndOfFile()
        {
            int expectedNumberOfOccurences = _currentFlatFile.Definition.NumberOfOccurences;
            if (expectedNumberOfOccurences > 0)
            {
                if (expectedNumberOfOccurences == _numberOfOcurrencesForCurrentFile)
                {
                    _testRun.Add(new TestResult(ResultType.Success,
                        $"Number of records ({expectedNumberOfOccurences}) matched for file {_currentFlatFile.Definition.FileName}."));
                }
                else
                {
                    _testRun.Add(new TestResult(ResultType.Error,
                        $"Number of records did not match for file {_currentFlatFile.Definition.FileName}. Expected {expectedNumberOfOccurences}, found {_numberOfOcurrencesForCurrentFile}. "));
                }
            }
        }

        public void Run(Record record)
        {
            _numberOfOcurrencesForCurrentFile++;
        }
    }
}