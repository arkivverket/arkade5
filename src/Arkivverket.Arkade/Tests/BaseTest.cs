using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests
{
    public abstract class BaseTest
    {
        protected abstract TestResults Test(ArchiveExtraction archive);

        public TestResults RunTest(ArchiveExtraction archive)
        {
            DateTime start = DateTime.Now;
            TestResults results = Test(archive);
            DateTime stop = DateTime.Now;
            Console.WriteLine("Duration: " + stop.Subtract(start).TotalMilliseconds);

            return results;
        }
    }
}
