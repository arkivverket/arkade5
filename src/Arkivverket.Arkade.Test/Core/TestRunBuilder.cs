using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Test.Core
{
    public class TestRunBuilder
    {
        private long _durationMillis = 1234;
        private string _testCategory = "testCategory";
        private string _testDescription = "testDescription";
        private string _testName = "test1";
        private List<TestResult> _testResults = new List<TestResult>();
        private readonly TestType _testType = TestType.Content;


        public TestRun Build()
        {
            var testRun = new TestRun(_testName, _testType);
            testRun.TestCategory = _testCategory;
            testRun.TestDescription = _testDescription;
            testRun.TestDuration = _durationMillis;

            if (_testResults.Count == 0)
            {
                _testResults.Add(new TestResult(ResultType.Success, new Location("location"), "message"));
            }
            testRun.Results = _testResults;

            return testRun;
        }

        public TestRunBuilder WithTestName(string testName)
        {
            _testName = testName;
            return this;
        }

        public TestRunBuilder WithTestCategory(string testCategory)
        {
            _testCategory = testCategory;
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