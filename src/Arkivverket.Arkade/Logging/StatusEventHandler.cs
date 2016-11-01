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

        public void IssueOnFileProcessingStart(StatusEventArgFileProcessing statusEventArgFileProcessing)
        {
            OnFileProcessingStartEvent(statusEventArgFileProcessing);
        }

        public void IssueOnFileProcessingStop(StatusEventArgFileProcessing statusEventArgFileProcessing)
        {
            OnFileProcessingStopEvent(statusEventArgFileProcessing);
        }

        public void IssueOnRecordProcessingStart(StatusEventArgRecord statusEventArgRecord)
        {
            OnRecordProcessingStartEvent(statusEventArgRecord);
        }

        public void IssueOnNewTestRecord(StatusEventArgRecord statusEventArgRecord)
        {
            OnNewTestRecordEvent(statusEventArgRecord);
        }

        public void IssueOnNewArchiveInformation(StatusEventNewArchiveInformation statusEventArgNewArchiveInformation)
        {
            OnIssueOnNewArchiveInformation(statusEventArgNewArchiveInformation);
        }


        public event EventHandler<StatusEventArgument> StatusEvent;
        public event EventHandler<StatusEventArgFileProcessing> FileProcessStartEvent;
        public event EventHandler<StatusEventArgFileProcessing> FileProcessStopEvent;
        public event EventHandler<StatusEventArgRecord> RecordProcessStartEvent;
        public event EventHandler<StatusEventArgRecord> NewTestRecordEvent;
        public event EventHandler<StatusEventNewArchiveInformation> NewArchiveProcessEvent;

        protected virtual void OnStatusEvent(StatusEventArgument e)
        {
            var handler = StatusEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnFileProcessingStartEvent(StatusEventArgFileProcessing e)
        {
            var handler = FileProcessStartEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnFileProcessingStopEvent(StatusEventArgFileProcessing e)
        {
            var handler = FileProcessStopEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnRecordProcessingStartEvent(StatusEventArgRecord e)
        {
            var handler = RecordProcessStartEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnNewTestRecordEvent(StatusEventArgRecord e)
        {
            var handler = NewTestRecordEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnIssueOnNewArchiveInformation(StatusEventNewArchiveInformation e)
        {
            var handler = NewArchiveProcessEvent;
            handler?.Invoke(this, e);
        }


    }
}
