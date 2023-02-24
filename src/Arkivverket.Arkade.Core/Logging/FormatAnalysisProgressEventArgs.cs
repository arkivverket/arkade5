using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class FormatAnalysisProgressEventArgs : EventArgs
    {
        public long? TargetSize { get; }
        public long FileSize { get; }

        public FormatAnalysisProgressEventArgs(long? targetSize, long fileSize)
        {
            TargetSize = targetSize;
            FileSize = fileSize;
        }
    }
}
