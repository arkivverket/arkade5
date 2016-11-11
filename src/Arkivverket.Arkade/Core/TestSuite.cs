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

        public int FindNumberOfErrors()
        {
            int sum = 0;
            foreach (var testRun in TestRuns)
            {
                sum += testRun.FindNumberOfErrors();
            }
            return sum;
        }

    }
}