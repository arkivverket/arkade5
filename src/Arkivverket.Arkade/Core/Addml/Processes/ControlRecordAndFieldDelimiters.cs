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

        public TestRun GetTestRun()
        {
            return new TestRun(AddmlMessages.RecordLengthErrorTestName, TestType.Structure)
            {
                TestDescription = Messages.NumberOfRecordsWithFieldDelimiterErrorDescription,
                Results = _testResults
            };
        }
    }
}
