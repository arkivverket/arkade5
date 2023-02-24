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

        void RaiseEventFormatAnalysisStarted();
        void RaiseEventFormatAnalysisTotalFileCounterFinished(long targetSize);
        void RaiseEventFormatAnalysisProgressUpdated(long bytesRead);
        void RaiseEventFormatAnalysisFinished();

        void RaiseEventReadXmlEndElement();


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
        
        event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisStartedEvent;
        event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisTotalFileCounterFinishedEvent;
        event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisProgressUpdatedEvent;
        event EventHandler<FormatAnalysisProgressEventArgs> FormatAnalysisFinishedEvent;

        event EventHandler<EventArgs> ReadXmlEndElementEvent;
    }
}