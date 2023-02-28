using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class FormatAnalysisProgressEventArgs : EventArgs
    {
        public long FileSize { get; }

        public FormatAnalysisProgressEventArgs(long fileSize)
        {
            FileSize = fileSize;
        }
    }
}
