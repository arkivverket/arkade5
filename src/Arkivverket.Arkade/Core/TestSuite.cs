using Arkivverket.Arkade.Tests;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core
{
    public class TestSuite
    {
        public List<TestRun> TestRuns { get; }

        public TestSuite()
        {
            TestRuns = new List<TestRun>();
        }

        public void AddTestRun(TestRun testRun)
        {
            TestRuns.Add(testRun);
        }

    }
}