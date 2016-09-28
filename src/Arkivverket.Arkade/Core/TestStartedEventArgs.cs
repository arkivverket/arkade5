using System;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core
{
    public class TestStartedEventArgs : EventArgs
    {
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }

        public TestStartedEventArgs(ITest test)
        {
            TestName = test.GetName();
            StartTime = DateTime.Now;
        }
    }
}