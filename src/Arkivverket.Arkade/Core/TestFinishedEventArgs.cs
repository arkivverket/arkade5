using System;

namespace Arkivverket.Arkade.Core
{
    public class TestFinishedEventArgs : EventArgs
    {
        public string TestName { get; set; }
        public bool IsSuccess { get; set; }
        public string ResultMessage { get; set; }

        public TestFinishedEventArgs(TestRun testRun)
        {
            TestName = testRun.TestName;
            IsSuccess = testRun.IsSuccess();

            if (testRun.Results != null && testRun.Results.Count > 0)
                ResultMessage = testRun.Results[0].Message;

        }
    }
}