using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Logging
{
    public class StatusEventHandler : IStatusEventHandler
    {
        public void RaiseEventTestStarted(ITest test)
        {
           OnStatusEvent(new TestInformationEventArgs(test.GetName(),DateTime.Now, StatusTestExecution.TestStarted, false, string.Empty));
        }

        public void RaiseEventTestFinished(TestRun testRun)
        {
            string resultMessage = string.Empty;

            if (testRun.Results != null && testRun.Results.Count > 0)
                resultMessage = testRun.Results[0].Message;

            OnStatusEvent(new TestInformationEventArgs(testRun.TestName, DateTime.Now, StatusTestExecution.TestCompleted, testRun.IsSuccess(), resultMessage));
        }

        public void RaiseEventTestInformation(string testName, string testMessage, StatusTestExecution status, bool isSuccess)
        {
            OnStatusEvent(new TestInformationEventArgs(testName, DateTime.Now, status, isSuccess, testMessage));
        }

        public void RaiseEventFileProcessingStarted(FileProcessingStatusEventArgs fileProcessingStatusEventArgs)
        {
            OnFileProcessingStartEvent(fileProcessingStatusEventArgs);
        }

        public void RaiseEventFileProcessingFinished(FileProcessingStatusEventArgs fileProcessingStatusEventArgs)
        {
            OnFileProcessingStopEvent(fileProcessingStatusEventArgs);
        }

        public void RaiseEventRecordProcessingStart()
        {
            OnRecordProcessingStartEvent(EventArgs.Empty);
        }

        public void RaiseEventRecordProcessingStopped()
        {
            OnRecordProcessingFinishedEvent(EventArgs.Empty);
        }
     
        public void RaiseEventNewArchiveInformation(ArchiveInformationEventArgs archiveInformationEventArgArgs)
        {
            OnIssueOnNewArchiveInformation(archiveInformationEventArgArgs);
        }

        public event EventHandler<TestInformationEventArgs> StatusEvent;

        public event EventHandler<FileProcessingStatusEventArgs> FileProcessStartedEvent;
        public event EventHandler<FileProcessingStatusEventArgs> FileProcessFinishedEvent;

        public event EventHandler<EventArgs> RecordProcessingStartedEvent;
        public event EventHandler<EventArgs> RecordProcessingFinishedEvent;

        public event EventHandler<ArchiveInformationEventArgs> NewArchiveProcessEvent;

        protected virtual void OnStatusEvent(TestInformationEventArgs e)
        {
            var handler = StatusEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnFileProcessingStartEvent(FileProcessingStatusEventArgs e)
        {
            var handler = FileProcessStartedEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnFileProcessingStopEvent(FileProcessingStatusEventArgs e)
        {
            var handler = FileProcessFinishedEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnRecordProcessingStartEvent(EventArgs e)
        {
            var handler = RecordProcessingStartedEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnRecordProcessingFinishedEvent(EventArgs e)
        {
            var handler = RecordProcessingFinishedEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnIssueOnNewArchiveInformation(ArchiveInformationEventArgs e)
        {
            var handler = NewArchiveProcessEvent;
            handler?.Invoke(this, e);
        }


    }
}
