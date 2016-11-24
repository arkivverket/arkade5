using System;

namespace Arkivverket.Arkade.Logging
{
    public class FileProcessingStatusEventArgs : EventArgs
    {
        public string EventId { get; set; }
        public string FileName { get; set; }
        public bool FinishedProcessing { get; set; }
        public string NumberOfItemsProcessed { get; set; }

        public FileProcessingStatusEventArgs(string eventId, string fileName)
        {
            EventId = eventId;
            FileName = fileName;
        }

        public FileProcessingStatusEventArgs(string eventId, string fileName, bool finishedProcessing) : this(eventId, fileName)
        {
            FinishedProcessing = finishedProcessing;
        }
    }
}