using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;

namespace Arkivverket.Arkade.Tests
{
    public abstract class BaseNoark5Test : INoark5Test
    {
        protected readonly IArchiveContentReader ArchiveReader;

        protected TestRun TestResults;

        private TestType TestType { get; }

        protected BaseNoark5Test(TestType testType, IArchiveContentReader archiveReader)
        {
            ArchiveReader = archiveReader;
            TestType = testType;
        }

        protected virtual void Test(Archive archive) { }

        public abstract void OnReadStartElementEvent(object sender, ReadElementEventArgs e);
        public virtual void OnReadEndElementEvent(object sender, ReadElementEventArgs e)
        {
        }

        public virtual void OnReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        public virtual TestRun GetTestRun()
        {
            return new TestRun(GetName(), TestType);
        }

        public string GetName()
        {
            return Resources.Noark5Messages.ResourceManager.GetString(GetType().Name);
        }

        public TestRun RunTest(Archive archive)
        {
            TestResults = new TestRun(GetName(), TestType);

            var start = DateTime.Now;
            Test(archive);
            var stop = DateTime.Now;

            TestResults.TestDuration = (long) stop.Subtract(start).TotalMilliseconds;

            return TestResults;
        }

       

        protected void TestSuccess(ILocation location, string message)
        {
            AddTestResult(ResultType.Success, location, message);
        }

        protected void TestError(ILocation location, string message)
        {
            AddTestResult(ResultType.Error, location, message);
        }

        private void AddTestResult(ResultType resultType, ILocation location, string message)
        {
            TestResults.Add(new TestResult(resultType, location, message));
        }

        protected void AddAnalysisResult(string key, string value)
        {
            TestResults.AddAnalysisResult(key, value);
        }
    }

    public enum TestType
    {
        Structure,
        ContentAnalysis,
        ContentControl
    }
}