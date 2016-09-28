using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core
{
    public class TestEngine
    {
        private readonly ITestProvider _testProvider;

        public TestEngine(ITestProvider testProvider)
        {
            _testProvider = testProvider;
        }

        public List<TestRun> RunTestsOnArchive(Archive archive)
        {
            List<ITest> testsToRun = _testProvider.GetTestsForArchive(archive);

            var testResultsFromAllTests = new List<TestRun>();
            foreach (ITest test in testsToRun)
            {
                OnTestStarted(new TestStartedEventArgs(test));

                var testResults = test.RunTest(archive);

                OnTestResultsArrived(new TestResultsArrivedEventArgs(testResults));

                testResultsFromAllTests.Add(testResults);
            }
            return testResultsFromAllTests;
        }

        public event EventHandler<TestResultsArrivedEventArgs> TestResultsArrived;
        public event EventHandler<TestStartedEventArgs> TestStarted;

        protected virtual void OnTestResultsArrived(TestResultsArrivedEventArgs e)
        {
            var handler = TestResultsArrived;
            handler?.Invoke(this, e);
        }

        protected virtual void OnTestStarted(TestStartedEventArgs e)
        {
            var handler = TestStarted;
            handler?.Invoke(this, e);
        }

    }

    public class TestResultsArrivedEventArgs : EventArgs
    {
        public string TestName { get; set; }
        public bool IsSuccess { get; set; }

        public TestResultsArrivedEventArgs(TestRun results)
        {
            TestName = results.TestName;
            IsSuccess = results.IsSuccess();
        }
    }
}