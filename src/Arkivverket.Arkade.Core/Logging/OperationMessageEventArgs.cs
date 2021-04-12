using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class OperationMessageEventArgs : EventArgs
    {
        public string Id { get; set; }
        public DateTime StartTime { get; set; }
        public OperationMessageStatus Status { get; set; }
        public string Message { get; set; }

        public OperationMessageEventArgs(string id, DateTime startTime, OperationMessageStatus status, string message)
        {
            Id = id;
            StartTime = startTime;
            Status = status;
            Message = message;
        }
    }


    public enum OperationMessageStatus
    {
        Started,
        Ok,
        Error,
        Warning,
        Info
    }
}