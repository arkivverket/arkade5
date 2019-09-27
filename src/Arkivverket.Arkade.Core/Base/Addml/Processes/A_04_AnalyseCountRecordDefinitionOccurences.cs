using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_04_AnalyseCountRecordDefinitionOccurences : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 4);

        public const string Name = "Analyse_CountRecordDefinitionOccurences";

        private readonly Dictionary<RecordIndex, int> _numberOfRecords = new Dictionary<RecordIndex, int>();
        private readonly List<TestResult> _testResults = new List<TestResult>();

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
            return Messages.AnalyseCountRecordDefinitionOccurencesDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        protected override void DoRun(Record record)
        {
            RecordIndex key = record.Definition.GetIndex();
            if (!_numberOfRecords.ContainsKey(key))
            {
                _numberOfRecords.Add(key, 0);
            }
            _numberOfRecords[key] = _numberOfRecords[key] + 1;
        }

        protected override void DoRun(Field field)
        {
        }

        protected override void DoEndOfFile()
        {
            foreach (KeyValuePair<RecordIndex, int> item in _numberOfRecords)
            {
                _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromRecordIndex(item.Key),
                    string.Format(Messages.AnalyseCountRecordDefinitionOccurencesMessage, item.Value)));
            }
            _numberOfRecords.Clear();
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }
    }
}