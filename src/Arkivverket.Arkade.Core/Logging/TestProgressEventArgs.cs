using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class TestProgressEventArgs : EventArgs
    {
        public string TestProgress { get; }
        public bool HasFailed { get; }
        public string FailMessage { get; }

        public TestProgressEventArgs(string testProgress, bool hasFailed, string failMessage)
        {
            TestProgress = testProgress;
            HasFailed = hasFailed;
            FailMessage = failMessage;
        }
    }
}
