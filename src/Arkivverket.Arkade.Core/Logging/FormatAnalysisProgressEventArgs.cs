using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class FormatAnalysisProgressEventArgs : EventArgs
    {
        public long FileCounter { get; }
        public long TotalFiles { get; }

        public FormatAnalysisProgressEventArgs(long fileCounter, long totalFiles)
        {
            FileCounter = fileCounter;
            TotalFiles = totalFiles;
        }
    }
}
