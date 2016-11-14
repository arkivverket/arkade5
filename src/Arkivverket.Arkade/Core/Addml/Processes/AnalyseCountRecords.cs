using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseCountRecords : IAddmlProcess
    {
        public const string Name = "Analyse_CountRecords";

        private readonly TestRun _testRun;
        private FlatFile _currentFlatFile;
        private int _numberRecords;

        public AnalyseCountRecords()
        {
            _testRun = new TestRun(Name, TestType.Content);
        }

        public string GetName()
        {
            return Name;
        }

        public string GetDescription()
        {
            return Messages.AnalyseCountRecordsDescription;
        }

        public void Run(FlatFile flatFile)
        {
            _numberRecords = 0;
            _currentFlatFile = flatFile;
        }

        public void Run(Record record)
        {
            _numberRecords++;
        }

        public void Run(Field field)
        {
        }

        public void EndOfFile()
        {
            _testRun.Add(new TestResult(ResultType.Success, new Location(_currentFlatFile.Definition.FileName),
                string.Format(Messages.AnalyseCountRecordsMessage, _numberRecords)));
        }

        public TestRun GetTestRun()
        {
            return _testRun;
        }
    }
}