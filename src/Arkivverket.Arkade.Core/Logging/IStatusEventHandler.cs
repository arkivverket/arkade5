using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Logging
{
    public interface IStatusEventHandler
    {
        void RaiseEventOperationMessage(string identifier, string message, OperationMessageStatus status);

        void RaiseEventFileProcessingStarted(FileProcessingStatusEventArgs fileProcessingStatusEventArgs);
        void RaiseEventFileProcessingFinished(FileProcessingStatusEventArgs fileProcessingStatusEventArgs);

        void RaiseEventRecordProcessingStart();
        void RaiseEventRecordProcessingStopped();

        void RaiseEventNewArchiveInformation(ArchiveInformationEventArgs archiveInformationEventArgArgs);

        void RaiseEventTestProgressUpdated(string testProgress, bool hasFailed = false, string failMessage = null);

        void RaiseEventSiardValidationFinished(List<string> errors);
        
        event EventHandler<OperationMessageEventArgs> OperationMessageEvent;

        event EventHandler<OperationMessageEventArgs> TestStartedEvent;

        event EventHandler<OperationMessageEventArgs> TestFinishedEvent;
        
        event EventHandler<FileProcessingStatusEventArgs> FileProcessStartedEvent;
        event EventHandler<FileProcessingStatusEventArgs> FileProcessFinishedEvent;

        event EventHandler<EventArgs> RecordProcessingStartedEvent;
        event EventHandler<EventArgs> RecordProcessingFinishedEvent;

        event EventHandler<ArchiveInformationEventArgs> NewArchiveProcessEvent;
        
        event EventHandler<TestProgressEventArgs> TestProgressUpdatedEvent;

        event EventHandler<SiardValidationEventArgs> SiardValidationFinishedEvent;
    }
}