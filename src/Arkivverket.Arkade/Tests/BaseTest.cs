using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests
{
    public abstract class BaseTest
    {
        public BaseTest(TestType testType)
        {
            TestType = testType;
        }

        public TestType TestType { get; private set; }

        protected TestResults TestResults;

        protected abstract void Test(ArchiveExtraction archive);

        public TestResults RunTest(ArchiveExtraction archive)
        {
            TestResults = new TestResults(this.GetType().FullName, TestType);

            var start = DateTime.Now;
            Test(archive);
            var stop = DateTime.Now;
            var duration = stop.Subtract(start).TotalMilliseconds;

            Console.WriteLine("Duration: " + duration); // TODO: use logging mechanism instead

            TestResults.TestDuration = duration;

            return TestResults;
        }
    }

    public enum TestType
    {
        Structure,
        Content
    }
}