using System.Collections.Generic;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlRecordAndFieldDelimiters : IAddmlHardcodedProcess
    {
        private readonly List<TestResult> _testResults;

        public ControlRecordAndFieldDelimiters(List<TestResult> testResults)
        {
            _testResults = testResults;
        }

        public string GetName()
        {
            return AddmlMessages.RecordLengthErrorTestName;
        }

        public TestType GetTestType()
        {
            return TestType.Structure;
        }

        public string GetDescription()
        {
            return Messages.NumberOfRecordsWithFieldDelimiterErrorDescription;
        }

        public TestRun GetTestRun()
        {
            return new TestRun(GetName(), GetTestType())
            {
                TestDescription = GetDescription(),
                Results = _testResults
            };
        }
    }
}
