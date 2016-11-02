using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Logging
{
    public class TestInformationEventArgs : EventArgs
    {
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }
        public StatusTestExecution TestStatus { get; set; }
        public bool IsSuccess { get; set; }
        public string ResultMessage { get; set; }


        public TestInformationEventArgs(string testName, DateTime startTime, StatusTestExecution testStatus, bool isSuccess, string resultMessage)
        {
            TestName = testName;
            StartTime = startTime;
            TestStatus = testStatus;
            IsSuccess = isSuccess;
            ResultMessage = resultMessage;
        }
    }


    public enum StatusTestExecution
    {
        TestStarted,
        TestInProgress,
        TestCompleted,
        TestErrored
    }
}




