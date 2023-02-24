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

        public void RaiseEventTestProgressUpdated(string testProgress, bool hasFailed, string failMessage)
        {
            OnTestProgressUpdatedEvent(new TestProgressEventArgs(testProgress, hasFailed, failMessage));
        }

        public void RaiseEventSiardValidationFinished(List<string> errors)
        {
            OnSiardValidationFinishedEvent(new SiardValidationEventArgs(errors));
        }

        public void RaiseEventFormatAnalysisStarted()
        {
            OnFormatAnalysisStarted(default);
        }

        public void RaiseEventFormatAnalysisTotalFileCounterFinished(long targetSize)
        {
            OnFormatAnalysisTotalFileCounterFinished(new FormatAnalysisProgressEventArgs(targetSize, 0));
        }

        public void RaiseEventFormatAnalysisProgressUpdated(long fileSize)
        {
            OnFormatAnalysisProgressUpdated(new FormatAnalysisProgressEventArgs(null, fileSize));
        }

        public void RaiseEventFormatAnalysisFinished()
        {
            OnFormatAnalysisFinished(default);
        }

        public void RaiseEventFormatAnalysisError(string fileName, string message)
        {
            OnFormatAnalysisError(new FormatAnalysisErrorEventArgs(fileName, message));
        }

        public void RaiseEventReadXmlEndElement()
        {
            OnReadXmlEndElement(default);
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
        public event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisTotalFileCounterFinishedEvent;
        public event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisProgressUpdatedEvent;
        public event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisFinishedEvent;
        public event EventHandler<FormatAnalysisErrorEventArgs> FormatAnalysisErrorEvent;

        public event EventHandler<EventArgs> ReadXmlEndElementEvent;

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
    
        protected virtual void OnFormatAnalysisTotalFileCounterFinished(FormatAnalysisProgressEventArgs eventArgs)
        {
            EventHandler<FormatAnalysisProgressEventArgs> handler = FormatAnalysisTotalFileCounterFinishedEvent;
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

        protected virtual void OnFormatAnalysisError(FormatAnalysisErrorEventArgs eventArgs)
        {
            EventHandler<FormatAnalysisErrorEventArgs> handler = FormatAnalysisErrorEvent;
            handler?.Invoke(this, eventArgs);
        }

        protected virtual void OnReadXmlEndElement(EventArgs eventArgs)
        {
            EventHandler<EventArgs> handler = ReadXmlEndElementEvent;
            handler?.Invoke(this, eventArgs);
        }
    }
}
