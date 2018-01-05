using System.Collections.Generic;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Processes.Hardcoded
{
    public class ControlRecordAndFieldDelimiters : AddmlHardcodedProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.AddmlSup, 0); // TODO: Assign correct test number

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
    }
}
