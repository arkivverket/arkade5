using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core
{
    public class LogEntry
    {
        public DateTime Timestamp { get; }

        public string Message { get; }

        public LogEntry(DateTime timestamp, string message)
        {
            Timestamp = timestamp;
            Message = message;
        }

    }
}
