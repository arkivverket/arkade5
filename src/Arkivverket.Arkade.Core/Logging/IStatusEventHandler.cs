using System;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests;

namespace Arkivverket.Arkade.Core.Logging
{
    public interface IStatusEventHandler
    {
        void RaiseEventTestStarted(INoark5Test noark5Test);
        void RaiseEventTestFinished(TestRun testRun);

        void RaiseEventOperationMessage(string identifier, string message, OperationMessageStatus status);

        void RaiseEventFileProcessingStarted(FileProcessingStatusEventArgs fileProcessingStatusEventArgs);
        void RaiseEventFileProcessingFinished(FileProcessingStatusEventArgs fileProcessingStatusEventArgs);

        void RaiseEventRecordProcessingStart();
        void RaiseEventRecordProcessingStopped();

        void RaiseEventNewArchiveInformation(ArchiveInformationEventArgs archiveInformationEventArgArgs);

        event EventHandler<OperationMessageEventArgs> OperationMessageEvent;

        event EventHandler<OperationMessageEventArgs> TestStartedEvent;

        event EventHandler<OperationMessageEventArgs> TestFinishedEvent;
        
        event EventHandler<FileProcessingStatusEventArgs> FileProcessStartedEvent;
        event EventHandler<FileProcessingStatusEventArgs> FileProcessFinishedEvent;

        event EventHandler<EventArgs> RecordProcessingStartedEvent;
        event EventHandler<EventArgs> RecordProcessingFinishedEvent;

        event EventHandler<ArchiveInformationEventArgs> NewArchiveProcessEvent;
    }
}