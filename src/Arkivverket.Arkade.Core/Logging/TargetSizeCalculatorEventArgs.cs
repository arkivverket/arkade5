using System;

namespace Arkivverket.Arkade.Core.Logging
{
    public class TargetSizeCalculatorEventArgs : EventArgs
    {
        public long TargetSize { get; }

        public TargetSizeCalculatorEventArgs(long targetSize)
        {
            TargetSize = targetSize;
        }
    }
}
