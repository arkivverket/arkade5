using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
{
    public class TestSuiteTest
    {
        private readonly TestResult _testResultWithError = new TestResult(ResultType.Error, new Location(""), "feil");
        private readonly TestResult _testResultWithSuccess = new TestResult(ResultType.Success, new Location(""), "feil");

        [Fact]
        public void FindNumberOfErrorsShouldReturnOneError()
        {
            var testSuite = new TestSuite();
            var testRun = new TestRun("test with error", TestType.Content);
            testRun.Add(new TestResult(ResultType.Error, new Location(""), "feil"));
            testSuite.AddTestRun(testRun);

            testSuite.FindNumberOfErrors().Should().Be(1);
        }

        [Fact]
        public void FindNumberOfErrorsShouldReturnTwoErrors()
        {
            TestSuite testSuite = CreateTestSuite(_testResultWithError, _testResultWithError, _testResultWithSuccess);

            testSuite.FindNumberOfErrors().Should().Be(2);
        }

        [Fact]
        public void FindNumberOfErrorsShouldZeroErrorsWhenNoTestResults()
        {
            TestSuite testSuite = CreateTestSuite(null);

            testSuite.FindNumberOfErrors().Should().Be(0);
        }

        [Fact]
        public void FindNumberOfErrorsShouldZeroErrorsWhenOnlySuccessResults()
        {
            TestSuite testSuite = CreateTestSuite(_testResultWithSuccess, _testResultWithSuccess);

            testSuite.FindNumberOfErrors().Should().Be(0);
        }

        private static TestSuite CreateTestSuite(params TestResult[] testResults)
        {
            var testSuite = new TestSuite();
            var testRun = new TestRun("test with error", TestType.Content);
            if (testResults != null)
            {
                foreach (var testResult in testResults)
                {
                    testRun.Add(testResult);
                }
            }
            testSuite.AddTestRun(testRun);
            return testSuite;
        }
    }
}
