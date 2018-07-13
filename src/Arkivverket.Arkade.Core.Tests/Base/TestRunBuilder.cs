using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Tests.Base
{
    public class TestRunBuilder
    {
        private long _durationMillis = 1234;
        private string _testDescription = "testDescription";
        private TestId _testId = new TestId(TestId.TestKind.Unidentified, 0);
        private string _testName = "test1";
        private List<TestResult> _testResults = new List<TestResult>();
        private readonly TestType _testType = TestType.ContentAnalysis;


        public TestRun Build()
        {
            TestRun testRun = new ArkadeTestMock(_testId, _testName, _testType, _testDescription).GetTestRun();
            testRun.TestDuration = _durationMillis;

            if (_testResults.Count == 0)
            {
                _testResults.Add(new TestResult(ResultType.Success, new Location("location"), "message"));
            }
            testRun.Results = _testResults;

            return testRun;
        }

        public TestRunBuilder WithTestId(TestId testId)
        {
            _testId = testId;
            return this;
        }

        public TestRunBuilder WithTestName(string testName)
        {
            _testName = testName;
            return this;
        }

        public TestRunBuilder WithDurationMillis(long millis)
        {
            _durationMillis = millis;
            return this;
        }

        public TestRunBuilder WithTestResult(TestResult testResult)
        {
            _testResults.Add(testResult);
            return this;
        }


        public TestRunBuilder WithTestResults(List<TestResult> testResults)
        {
            _testResults = testResults;
            return this;
        }

        public TestRunBuilder WithTestDescription(string testDescription)
        {
            _testDescription = testDescription;
            return this;
        }
    }
}