using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core
{
    public class TestEngine
    {
        public List<TestResults> RunTestsOnArchive(ArchiveExtraction archiveExtraction)
        {
            var testsToRun = new TestProvider().GetTestsForArchiveExtraction(archiveExtraction);

            var testResultsFromAllTests = new List<TestResults>();
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
        public TestResultsArrivedEventArgs(TestResults results)
        {
            TestName = results.TestName;
            IsSuccess = results.IsSuccess();
        }

        public string TestName { get; set; }
        public bool IsSuccess { get; set; }
    }
}