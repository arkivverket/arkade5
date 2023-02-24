using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class FormatAnalysisErrorEventArgs : EventArgs
    {
        public string FileName { get; }
        public string Message { get; }

        public FormatAnalysisErrorEventArgs(string fileName, string message)
        {
            FileName = fileName;
            Message = message;
        }
    }
}
