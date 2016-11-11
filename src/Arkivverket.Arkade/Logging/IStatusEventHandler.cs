using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Logging
{
    public interface IStatusEventHandler
    {
        void RaiseEventTestStarted(ITest test);
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