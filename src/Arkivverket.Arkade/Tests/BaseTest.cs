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

        protected abstract TestResults Test(ArchiveExtraction archive);

        public TestResults RunTest(ArchiveExtraction archive)
        {
            var start = DateTime.Now;
            var results = Test(archive);
            var stop = DateTime.Now;
            Console.WriteLine("Duration: " + stop.Subtract(start).TotalMilliseconds);

            return results;
        }
    }

    public enum TestType
    {
        Structure,
        Content
    }
}