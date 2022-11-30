﻿using System;
using System.Collections.Generic;
using System.IO;

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

        public void RaiseEventTestProgressUpdated(string testProgress, bool hasFailed, string failMessage)
        {
            OnTestProgressUpdatedEvent(new TestProgressEventArgs(testProgress, hasFailed, failMessage));
        }

        public void RaiseEventSiardValidationFinished(List<string> errors)
        {
            OnSiardValidationFinishedEvent(new SiardValidationEventArgs(errors));
        }

        public void RaiseEventFormatAnalysisStarted(long totalAmountOfFiles)
        {
            OnFormatAnalysisStarted(new FormatAnalysisProgressEventArgs(totalAmountOfFiles));
        }

        public void RaiseEventFormatAnalysisProgressUpdated()
        {
            OnFormatAnalysisProgressUpdated(default);
        }

        public void RaiseEventFormatAnalysisFinished()
        {
            OnFormatAnalysisFinished(default);
        }

        public void RaiseEventIoAccessLost(DirectoryInfo writeLocation, IoAccessType ioAccessType)
        {
            OnIoAccessLost(new IoAccessEventArgs(writeLocation, ioAccessType));
        }

        public void RaiseEventAbortExecution(DirectoryInfo location, IoAccessType ioAccessType)
        {
            OnAbortExecution(new IoAccessEventArgs(location, ioAccessType));
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

        public event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisStartedEvent;
        public event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisProgressUpdatedEvent;
        public event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisFinishedEvent;

        public event EventHandler<IoAccessEventArgs> IoAccessLostEvent;
        public event EventHandler<IoAccessEventArgs> AbortExecutionEvent;

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

        protected virtual void OnFormatAnalysisStarted(FormatAnalysisProgressEventArgs eventArgs)
        {
            EventHandler<FormatAnalysisProgressEventArgs> handler = FormatAnalysisStartedEvent;
            handler?.Invoke(this, eventArgs);
        }
    
        protected virtual void OnFormatAnalysisProgressUpdated(FormatAnalysisProgressEventArgs eventArgs)
        {
            EventHandler<FormatAnalysisProgressEventArgs> handler = FormatAnalysisProgressUpdatedEvent;
            handler?.Invoke(this, eventArgs);
        }
    
        protected virtual void OnFormatAnalysisFinished(FormatAnalysisProgressEventArgs eventArgs)
        {
            EventHandler<FormatAnalysisProgressEventArgs> handler = FormatAnalysisFinishedEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnIoAccessLost(IoAccessEventArgs eventArgs)
        {
            EventHandler<IoAccessEventArgs> handler = IoAccessLostEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnAbortExecution(IoAccessEventArgs eventArgs)
        {
            EventHandler<IoAccessEventArgs> handler = AbortExecutionEvent;
            handler?.Invoke(this, eventArgs);
        }
    }
}
