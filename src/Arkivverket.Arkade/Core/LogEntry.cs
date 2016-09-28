using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core
{
    class LogEntry
    {
        private DateTime Timestamp { get; }

        private string Message { get; }

        public LogEntry(DateTime timestamp, string message)
        {
            Timestamp = timestamp;
            Message = message;
        }

    }
}
