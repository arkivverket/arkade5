using Arkivverket.Arkade.Core;
using System.Collections.Generic;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Test.Core
{
    public class TestRunBuilder
    {
        private string _testName = "test1";
        private string _testCategory = "testCategory";
        private string _testDescription = "testDescription";
        private long _durationMillis = 1234;
        private ResultType _status = ResultType.Success;
        private string _message = "message";

        private TestType _testType = TestType.Content;

        public TestRun Build()
        {
            var testRun = new TestRun(_testName, _testType);
            testRun.TestCategory = _testCategory;
            testRun.TestDescription = _testDescription;
            testRun.TestDuration = _durationMillis;

            testRun.Results = new List<TestResult>()
            {
                new TestResult(_status, _message)
            };

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

        public TestRunBuilder WithStatus(ResultType status)
        {
            _status = status;
            return this;
        }

        public TestRunBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public TestRunBuilder WithDurationMillis(long millis)
        {
            _durationMillis = millis;
            return this;
        }
    }
}
