using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.LogInterface
{
    public class LogEntry
    {

        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public LogSubsystem Subsystem { get; set; }
        public LogLevel Level { get; set; }

        public LogEntry(string message, LogSubsystem subsystem, LogLevel level)
        {
            Timestamp = DateTime.Now;
            Message = message;
            Subsystem = subsystem;
            Level = level;
        }

    }


    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Critical,
        Fatal,
        Event
    }


    public enum LogSubsystem
    {
        Validator,
        Unpack,
        StateChange
    }

}
