using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseCountRecordDefinitionOccurences : IAddmlProcess
    {
        public const string Name = "Analyse_CountRecordDefinitionOccurences";
        public const string Description = "Teller opp antall poster for hver posttype";

        private readonly Dictionary<RecordIndex, int> _numberOfRecords = new Dictionary<RecordIndex, int>();
        private readonly TestRun _testRun;

        public AnalyseCountRecordDefinitionOccurences()
        {
            _testRun = new TestRun(Name, TestType.Content);
        }

        public string GetName()
        {
            return Name;
        }

        public string GetDescription()
        {
            return Description;
        }

        public void Run(FlatFile flatFile)
        {
        }

        public void Run(Field field)
        {
        }

        public void Run(Record record)
        {
            RecordIndex key = record.Definition.GetIndex();
            if (!_numberOfRecords.ContainsKey(key))
            {
                _numberOfRecords.Add(key, 0);
            }
            _numberOfRecords[key] = _numberOfRecords[key] + 1;
        }

        public void EndOfFile()
        {
            foreach (KeyValuePair<RecordIndex, int> item in _numberOfRecords)
            {
                _testRun.Add(new TestResult(ResultType.Success, AddmlLocation.FromRecordIndex(item.Key), item.Value + " " + Messages.Records));
            }
            _numberOfRecords.Clear();
        }

        public TestRun GetTestRun()
        {
            return _testRun;
        }
    }
}