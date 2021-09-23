using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Logging
{
    public class StatusEventHandler : IStatusEventHandler
    {
        public void RaiseEventOperationMessage(string identifier, string message, OperationMessageStatus status)
        {
            OnOperationMessageEvent(new OperationMessageEventArgs(identifier, DateTime.Now, status, message));
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

        public void RaiseEventTestProgressUpdated(string testProgressValueWithUnit)
        {
            OnTestProgressUpdatedEvent(new TestProgressEventArgs(testProgressValueWithUnit));
        }

        public void RaiseEventSiardValidationFinished(List<string> errors, int numberOfErrors, int numberOfWarnings)
        {
            OnSiardValidationFinishedEvent(new SiardValidationEventArgs(errors, numberOfErrors, numberOfWarnings));
        }

        public event EventHandler<OperationMessageEventArgs> OperationMessageEvent;

        public event EventHandler<OperationMessageEventArgs> TestStartedEvent;

        public event EventHandler<OperationMessageEventArgs> TestFinishedEvent;

        public event EventHandler<FileProcessingStatusEventArgs> FileProcessStartedEvent;
        public event EventHandler<FileProcessingStatusEventArgs> FileProcessFinishedEvent;

        public event EventHandler<EventArgs> RecordProcessingStartedEvent;
        public event EventHandler<EventArgs> RecordProcessingFinishedEvent;

        public event EventHandler<ArchiveInformationEventArgs> NewArchiveProcessEvent;
        public event EventHandler<TestProgressEventArgs> TestProgressUpdatedEvent;

        public event EventHandler<SiardValidationEventArgs> SiardValidationFinishedEvent;

        private void OnTestStartedEvent(OperationMessageEventArgs eventArgs)
        {
            var handler = TestStartedEvent;
            handler?.Invoke(this, eventArgs);
        }

        private void OnTestFinishedEvent(OperationMessageEventArgs eventArgs)
        {
            var handler = TestFinishedEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnOperationMessageEvent(OperationMessageEventArgs eventArgs)
        {
            var handler = OperationMessageEvent;
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
        
        protected virtual void OnTestProgressUpdatedEvent(TestProgressEventArgs eventArgs)
        {
            var handler = TestProgressUpdatedEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnSiardValidationFinishedEvent(SiardValidationEventArgs eventArgs)
        {
            var handler = SiardValidationFinishedEvent;
            handler?.Invoke(this, eventArgs);
        }
    }
}
