using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core
{
    public class TestEngine : ITestEngine
    {
        private readonly ITestProvider _testProvider;
        private readonly IStatusEventHandler _statusEventHandler;

        public TestEngine(ITestProvider testProvider, StatusEventHandler statusEventHandler)
        {
            _testProvider = testProvider;
            _statusEventHandler = statusEventHandler;
        }

        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
            List<ITest> testsToRun = _testProvider.GetTestsForArchive(testSession.Archive);

            var testSuite = new TestSuite();
            
            foreach (ITest test in testsToRun)
            {
                _statusEventHandler.IssueOnTestStarted(test);

                var testRun = test.RunTest(testSession.Archive);

                _statusEventHandler.IssueOnTestFinsihed(testRun);

                testSuite.AddTestRun(testRun);
            }
            return testSuite;
        }
    }
}