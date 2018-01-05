using System.Collections.Generic;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Processes.Hardcoded
{
    public class ControlRecordAndFieldDelimiters : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.AddmlHardcoded, 3);

        private readonly List<TestResult> _testResults;

        public ControlRecordAndFieldDelimiters(List<TestResult> testResults)
        {
            _testResults = testResults;
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return AddmlMessages.RecordLengthErrorTestName;
        }

        public override TestType GetTestType()
        {
            return TestType.Structure;
        }

        public override string GetDescription()
        {
            return Messages.NumberOfRecordsWithFieldDelimiterErrorDescription;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoRun(Field field)
        {
        }

        protected override void DoEndOfFile()
        {
        }
    }
}
