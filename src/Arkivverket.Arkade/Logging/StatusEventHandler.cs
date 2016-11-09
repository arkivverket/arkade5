using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Logging
{
    public class StatusEventHandler : IStatusEventHandler
    {
        public void RaiseEventTestStarted(ITest test)
        {
            OnTestStartedEvent(new TestInformationEventArgs(test.GetName(),DateTime.Now, StatusTestExecution.TestStarted, false, string.Empty));
        }
       
        public void RaiseEventTestFinished(TestRun testRun)
        {
            OnTestFinishedEvent(new TestInformationEventArgs(testRun.TestName, DateTime.Now, StatusTestExecution.TestCompleted, testRun.IsSuccess(), testRun.ToString()));
        }

        public void RaiseEventTestInformation(string identifier, string message, StatusTestExecution status, bool isSuccess)
        {
            OnStatusEvent(new TestInformationEventArgs(identifier, DateTime.Now, status, isSuccess, message));
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

        public event EventHandler<TestInformationEventArgs> TestStartedEvent;

        public event EventHandler<TestInformationEventArgs> TestFinishedEvent;

        public event EventHandler<FileProcessingStatusEventArgs> FileProcessStartedEvent;
        public event EventHandler<FileProcessingStatusEventArgs> FileProcessFinishedEvent;

        public event EventHandler<EventArgs> RecordProcessingStartedEvent;
        public event EventHandler<EventArgs> RecordProcessingFinishedEvent;

        public event EventHandler<ArchiveInformationEventArgs> NewArchiveProcessEvent;

        private void OnTestStartedEvent(TestInformationEventArgs eventArgs)
        {
            var handler = TestStartedEvent;
            handler?.Invoke(this, eventArgs);
        }

        private void OnTestFinishedEvent(TestInformationEventArgs eventArgs)
        {
            var handler = TestFinishedEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnStatusEvent(TestInformationEventArgs eventArgs)
        {
            var handler = StatusEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnFileProcessingStartEvent(FileProcessingStatusEventArgs eventArgs)
        {
            var handler = FileProcessStartedEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnFileProcessingStopEvent(FileProcessingStatusEventArgs eventArgs)
        {
            var handler = FileProcessFinishedEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnRecordProcessingStartEvent(EventArgs eventArgs)
        {
            var handler = RecordProcessingStartedEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnRecordProcessingFinishedEvent(EventArgs eventArgs)
        {
            var handler = RecordProcessingFinishedEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnIssueOnNewArchiveInformation(ArchiveInformationEventArgs eventArgs)
        {
            var handler = NewArchiveProcessEvent;
            handler?.Invoke(this, eventArgs);
        }


    }
}
