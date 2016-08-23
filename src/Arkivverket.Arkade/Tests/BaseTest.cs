using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests
{
    public abstract class BaseTest
    {
        protected abstract void Test(ArchiveExtraction archive);

        public void RunTest(ArchiveExtraction archive)
        {
            DateTime start = DateTime.Now;
            Test(archive);
            DateTime stop = DateTime.Now;
            Console.WriteLine("Duration: " + stop.Subtract(start).TotalMilliseconds);
        }
    }
}
