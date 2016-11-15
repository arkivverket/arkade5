using System.Collections.Generic;
using System.Diagnostics;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public abstract class AddmlProcess : IAddmlProcess
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public abstract string GetName();
        public abstract string GetDescription();
        public abstract TestType GetTestType();

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

        public TestRun GetTestRun()
        {
            _stopwatch.Start();
            List<TestResult> testRunResults = GetTestResults();
            _stopwatch.Stop();

            TestRun testRun = new TestRun(GetName(), GetTestType());
            testRun.TestDuration = _stopwatch.ElapsedMilliseconds;
            testRun.TestDescription = GetDescription();
            //testRun.TestCategory = GetCategory();
            testRun.Results = testRunResults;

            return testRun;
        }

        protected abstract void DoEndOfFile();

        protected abstract List<TestResult> GetTestResults();

        protected abstract void DoRun(FlatFile flatFile);

        protected abstract void DoRun(Record record);

        protected abstract void DoRun(Field field);
    }
}