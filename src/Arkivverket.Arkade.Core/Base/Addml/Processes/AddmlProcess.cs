using System.Collections.Generic;
using System.Diagnostics;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public abstract class AddmlProcess : IAddmlProcess
    {
        public abstract TestId GetId();
        public abstract string GetName();
        public abstract string GetDescription();
        public abstract TestType GetTestType();
        protected abstract List<TestResult> GetTestResults();
        protected abstract void DoRun(FlatFile flatFile);
        protected abstract void DoRun(Record record);
        protected abstract void DoRun(Field field);
        protected abstract void DoEndOfFile();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public TestRun GetTestRun()
        {
            _stopwatch.Start();
            List<TestResult> testRunResults = GetTestResults();
            _stopwatch.Stop();

            var testResultSet = new TestResultSet {TestsResults = testRunResults};

            return new TestRun(this)
            {
                TestDuration = _stopwatch.ElapsedMilliseconds,
                TestResults = testResultSet
            };
        }

        public void Run(FlatFile flatFile)
        {
            _stopwatch.Start();
            DoRun(flatFile);
            _stopwatch.Stop();
        }

        public void Run(Record record)
        {
            _stopwatch.Start();
            DoRun(record);
            _stopwatch.Stop();
        }

        public void Run(Field field)
        {
            _stopwatch.Start();
            DoRun(field);
            _stopwatch.Stop();
        }

        public void EndOfFile()
        {
            _stopwatch.Start();
            DoEndOfFile();
            _stopwatch.Stop();
        }

        public int CompareTo(object obj)
        {
            var arkadeTest = (IArkadeTest) obj;

            return GetId().CompareTo(arkadeTest.GetId());
        }
    }
}
