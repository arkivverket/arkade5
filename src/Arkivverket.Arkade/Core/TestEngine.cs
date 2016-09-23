using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core
{
    public class TestEngine
    {
        private readonly TestProvider _testProvider;

        public TestEngine(TestProvider testProvider)
        {
            _testProvider = testProvider;
        }

        public List<TestRun> RunTestsOnArchive(Archive archiveExtraction)
        {
            var testsToRun = _testProvider.GetTestsForArchiveExtraction(archiveExtraction);

            var testResultsFromAllTests = new List<TestRun>();
            foreach (var test in testsToRun)
            {
                var testResults = test.RunTest(archiveExtraction);

                OnTestResultsArrived(new TestResultsArrivedEventArgs(testResults));

                testResultsFromAllTests.Add(testResults);
            }
            return testResultsFromAllTests;
        }

        public event EventHandler<TestResultsArrivedEventArgs> TestResultsArrived;

        protected virtual void OnTestResultsArrived(TestResultsArrivedEventArgs e)
        {
            var handler = TestResultsArrived;
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