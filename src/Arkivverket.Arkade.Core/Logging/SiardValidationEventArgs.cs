using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Logging
{
    public class SiardValidationEventArgs : EventArgs
    {
        public List<string> Errors { get; }

        public SiardValidationEventArgs(List<string> errors)
        {
            Errors = errors;
        }
    }
}
