using System;
using Arkivverket.Arkade.Core.Testing;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestStartedEventArgs : EventArgs
    {
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }

        public TestStartedEventArgs(INoark5Test noark5Test)
        {
            TestName = ArkadeTestInfoProvider.GetDisplayName(noark5Test);
            StartTime = DateTime.Now;
        }

        public TestStartedEventArgs(string testName)
        {
            TestName = testName;
            StartTime = DateTime.Now;
        }
    }
}