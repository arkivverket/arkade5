using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using System.Linq;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlNotUsedRecordDef : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 0); // TODO: Assign correct test number

        public const string Name = "Control_NotUsedRecordDef";

        private readonly Dictionary<RecordIndex, bool> _recordDefinitionsInUse 
            = new Dictionary<RecordIndex, bool>();

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
            return Messages.ControlNotUsedRecordDefDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }

        protected override void DoRun(FlatFile flatFile)
        {
            foreach (AddmlRecordDefinition recordDefinition in flatFile.Definition.AddmlRecordDefinitions)
            {
                _recordDefinitionsInUse.Add(recordDefinition.GetIndex(), false);
            }
        }

        protected override void DoRun(Record record)
        {
            RecordIndex recordIndex = record.Definition.GetIndex();
            _recordDefinitionsInUse[recordIndex] = true;
        }

        protected override void DoEndOfFile()
        {
            foreach (KeyValuePair<RecordIndex, bool> entry in _recordDefinitionsInUse)
            {
                RecordIndex index = entry.Key;
                bool isInUse = entry.Value;
                
                if (!isInUse)
                {
                    _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromRecordIndex(entry.Key),
                        Messages.ControlNotUsedRecordDefMessage));
                }
            }

            _recordDefinitionsInUse.Clear();
        }

        protected override void DoRun(Field field)
        {
        }
    }


}