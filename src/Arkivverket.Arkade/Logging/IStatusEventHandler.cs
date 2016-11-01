using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Logging
{
    public interface IStatusEventHandler
    {
        void IssueOnTestStarted(ITest test);
        void IssueOnTestFinsihed(TestRun testRun);
        void IssueOnTestInformation(string testName, string testMessage, StatusTestExecution status, bool isSuccess);
        void IssueOnFileProcessingStart(StatusEventArgFileProcessing statusEventArgFileProcessing);
        void IssueOnFileProcessingStop(StatusEventArgFileProcessing statusEventArgFileProcessing);
        void IssueOnRecordProcessingStart(StatusEventArgRecord statusEventArgRecord);
        void IssueOnNewTestRecord(StatusEventArgRecord statusEventArgRecord);
        void IssueOnNewArchiveInformation(StatusEventNewArchiveInformation statusEventArgNewArchiveInformation);
        event EventHandler<StatusEventArgument> StatusEvent;
        event EventHandler<StatusEventArgFileProcessing> FileProcessStartEvent;
        event EventHandler<StatusEventArgFileProcessing> FileProcessStopEvent;
        event EventHandler<StatusEventArgRecord> RecordProcessStartEvent;
        event EventHandler<StatusEventArgRecord> NewTestRecordEvent;
        event EventHandler<StatusEventNewArchiveInformation> NewArchiveProcessEvent;

    }
}
