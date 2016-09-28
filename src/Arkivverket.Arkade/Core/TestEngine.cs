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

        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
            List<ITest> testsToRun = _testProvider.GetTestsForArchive(testSession.Archive);

            var testSuite = new TestSuite();
            
            foreach (ITest test in testsToRun)
            {
                OnTestStarted(new TestStartedEventArgs(test));

                var testRun = test.RunTest(testSession.Archive);

                OnTestFinished(new TestFinishedEventArgs(testRun));

                testSuite.AddTestRun(testRun);
            }
            return testSuite;
        }

        public event EventHandler<TestFinishedEventArgs> TestFinished;
        public event EventHandler<TestStartedEventArgs> TestStarted;

        protected virtual void OnTestFinished(TestFinishedEventArgs e)
        {
            var handler = TestFinished;
            handler?.Invoke(this, e);
        }

        protected virtual void OnTestStarted(TestStartedEventArgs e)
        {
            var handler = TestStarted;
            handler?.Invoke(this, e);
        }

    }

    public class TestFinishedEventArgs : EventArgs
    {
        public string TestName { get; set; }
        public bool IsSuccess { get; set; }

        public TestFinishedEventArgs(TestRun results)
        {
            TestName = results.TestName;
            IsSuccess = results.IsSuccess();
        }
    }
}