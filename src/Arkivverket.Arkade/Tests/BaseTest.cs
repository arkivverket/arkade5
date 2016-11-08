using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests
{
    public abstract class BaseTest : ITest
    {
        protected readonly IArchiveContentReader ArchiveReader;

        protected TestRun TestResults;

        private TestType TestType { get; }

        protected BaseTest(TestType testType, IArchiveContentReader archiveReader)
        {
            ArchiveReader = archiveReader;
            TestType = testType;
        }

        protected abstract void Test(Archive archive);

        public string GetName()
        {
            return Resources.TestNames.ResourceManager.GetString(GetType().Name);
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
        Content
    }
}