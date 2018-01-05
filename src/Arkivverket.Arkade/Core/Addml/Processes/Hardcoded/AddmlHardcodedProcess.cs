using System.Collections.Generic;
using System.Diagnostics;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Processes.Hardcoded
{
    public abstract class AddmlHardcodedProcess : IAddmlHardcodedProcess
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public abstract TestId GetId();
        public abstract string GetName();
        public abstract string GetDescription();
        public abstract TestType GetTestType();
        protected abstract List<TestResult> GetTestResults();

        public TestRun GetTestRun()
        {
            _stopwatch.Start();
            List<TestResult> testResults = GetTestResults();
            _stopwatch.Stop();

            return new TestRun(this)
            {
                TestDuration = _stopwatch.ElapsedMilliseconds,
                Results = testResults
            };
        }
    }
}
