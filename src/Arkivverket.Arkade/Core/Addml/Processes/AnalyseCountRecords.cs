using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseCountRecords : IAddmlProcess
    {

        public const string Name = "Analyse_CountRecords";
        private readonly TestRun _testRun;
        private int _numberRecords;
        private FlatFile _currentFlatFile;

        public AnalyseCountRecords()
        {
            _testRun = new TestRun(Name, TestType.Content);
        }

        public string GetName()
        {
            return Name;
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
            _testRun.Add(new TestResult(ResultType.Success, new Location(_currentFlatFile.Definition.FileName), $"RecordCount {_numberRecords}."));
        }

        public TestRun GetTestRun()
        {
            return _testRun;
        }
    }
}
