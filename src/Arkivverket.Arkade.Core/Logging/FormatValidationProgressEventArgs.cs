using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class FormatValidationProgressEventArgs : EventArgs
    {
        public long TotalFiles { get; }

        public FormatValidationProgressEventArgs(long totalFiles)
        {
            TotalFiles = totalFiles;
        }
    }
}
