using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class FormatAnalysisProgressEventArgs : EventArgs
    {
        public long TotalFiles { get; }

        public FormatAnalysisProgressEventArgs(long totalFiles)
        {
            TotalFiles = totalFiles;
        }
    }
}
