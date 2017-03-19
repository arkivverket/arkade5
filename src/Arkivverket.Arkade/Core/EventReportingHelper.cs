using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Logging;

namespace Arkivverket.Arkade.Core
{
    public class EventReportingHelper
    {

        private readonly IStatusEventHandler _statusEventHandler;

        public EventReportingHelper(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
        }


        public void RaiseEventOperationMessage(string identifier, string message, OperationMessageStatus status)
        {
            _statusEventHandler.RaiseEventOperationMessage(identifier, message, status);
        }

    }
}
