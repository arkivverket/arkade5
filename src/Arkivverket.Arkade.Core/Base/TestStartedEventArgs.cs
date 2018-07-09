using System;
using Arkivverket.Arkade.Core.Tests;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestStartedEventArgs : EventArgs
    {
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }

        public TestStartedEventArgs(INoark5Test noark5Test)
        {
            TestName = noark5Test.GetName();
            StartTime = DateTime.Now;
        }

        public TestStartedEventArgs(string testName)
        {
            TestName = testName;
            StartTime = DateTime.Now;
        }
    }
}