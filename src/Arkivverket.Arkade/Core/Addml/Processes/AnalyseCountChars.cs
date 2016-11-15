using System.Numerics;
using System.Windows.Forms;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseCountChars : IAddmlProcess
    {
        public const string Name = "Analyse_CountChars";

        private readonly TestRun _testRun;
        private FlatFile _currentFlatFile;
        private BigInteger _numberOfChars;

        public AnalyseCountChars()
        {
            _testRun = new TestRun(Name, TestType.Content);
        }

        public string GetName()
        {
            return Name;
        }

        public string GetDescription()
        {
            return Messages.AnalyseCountCharsDescription;
        }

        public void Run(FlatFile flatFile)
        {
            _numberOfChars = 0;
            _currentFlatFile = flatFile;
        }

        public void Run(Record record)
        {
        }

        public void Run(Field field)
        {
            _numberOfChars += field.Value.Length;
        }

        public void EndOfFile()
        {
            _testRun.Add(new TestResult(ResultType.Success, new Location(_currentFlatFile.Definition.FileName),
                string.Format(Messages.AnalyseCountCharsMessage, _numberOfChars)));
        }

        public TestRun GetTestRun()
        {
            return _testRun;
        }
    }
}