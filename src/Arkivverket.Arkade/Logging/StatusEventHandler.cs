using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Logging
{
    public class StatusEventHandler : IStatusEventHandler
    {

        public StatusEventHandler()
        {
            
        }


        public void IssueOnTestStarted(ITest test)
        {
           OnStatusEvent(new StatusEventArgument(test.GetName(),DateTime.Now, StatusTestExecution.TestStarted, false, string.Empty));
        }


        public void IssueOnTestFinsihed(TestRun testRun)
        {
            string resultMessage = string.Empty;

            if (testRun.Results != null && testRun.Results.Count > 0)
                resultMessage = testRun.Results[0].Message;

            OnStatusEvent(new StatusEventArgument(testRun.TestName, DateTime.Now, StatusTestExecution.TestCompleted, testRun.IsSuccess(), resultMessage));
        }


        public void IssueOnTestInformation(string testName, string testMessage, StatusTestExecution status, bool isSuccess)
        {
            OnStatusEvent(new StatusEventArgument(testName, DateTime.Now, status, isSuccess, testMessage));
        }

        public event EventHandler<StatusEventArgument> StatusEvent;

        protected virtual void OnStatusEvent(StatusEventArgument e)
        {
            var handler = StatusEvent;
            handler?.Invoke(this, e);
        }


    }
}
