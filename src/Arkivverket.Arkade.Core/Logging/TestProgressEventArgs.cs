using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class TestProgressEventArgs : EventArgs
    {
        public string TestProgressValueWithUnit { get; }

        public TestProgressEventArgs(string testTestProgressValueAndUnit)
        {
            TestProgressValueWithUnit = testTestProgressValueAndUnit;
        }
    }
}
