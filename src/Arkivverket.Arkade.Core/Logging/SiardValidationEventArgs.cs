using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Logging
{
    public class SiardValidationEventArgs : EventArgs
    {
        public int NumberOfErrors { get; }
        public int NumberOfWarnings { get; }
        public List<string> Errors { get; }

        public SiardValidationEventArgs(List<string> errors, int numberOfErrors, int numberOfWarnings)
        {
            Errors = errors;
            NumberOfErrors = numberOfErrors;
            NumberOfWarnings = numberOfWarnings;
        }
    }
}
