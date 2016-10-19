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
        event EventHandler<StatusEventArgument> StatusEvent;
    }
}
