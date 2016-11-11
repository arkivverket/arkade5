using Arkivverket.Arkade.Core;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Test.Core
{
    public class TestSessionBuilder
    {

        private Archive _archive;
        private List<string> _logEntries = new List<string>();
        private List<TestRun> _testRuns = new List<TestRun>();
        private TestSummary _testSummary;

        public TestSessionBuilder WithLogEntry(string message)
        {
            _logEntries.Add(message);
            return this;
        }
        public TestSessionBuilder WithTestRuns(List<TestRun> testRuns)
        {
            _testRuns = testRuns;
            return this;
        }

        public TestSessionBuilder WithTestRun(TestRun testRun)
        {
            _testRuns.Add(testRun);
            return this;
        }

        public TestSessionBuilder WithArchive(Archive archive)
        {
            _archive = archive;
            return this;
        }

        public TestSessionBuilder WithTestSummary(TestSummary testSummary)
        {
            _testSummary = testSummary;
            return this;
        }

        public TestSession Build()
        {
            if (_archive == null)
            {
                _archive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark3).Build();
            }

            var testSession = new TestSession(_archive);
            foreach (var logEntry in _logEntries)
            {
                testSession.AddLogEntry(logEntry);
            }

            TestSuite testSuite = new TestSuite();
            if (_testRuns.Count != 0)
            {
                
                foreach (var testRun in _testRuns)
                {
                    testSuite.AddTestRun(testRun);
                }
                
            } else
            {
                testSuite.AddTestRun(new TestRunBuilder().Build());
            }
            testSession.TestSuite = testSuite;

            if (_testSummary == null)
                _testSummary = new TestSummary(0,0,0);

            testSession.TestSummary = _testSummary;

            return testSession;
        }

    }
}
