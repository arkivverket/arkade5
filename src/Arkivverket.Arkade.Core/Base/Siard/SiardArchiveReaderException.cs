using System;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardArchiveReaderException : Exception
    {
        public SiardArchiveReaderException()
        {
        }

        public SiardArchiveReaderException(string message)
            : base(message)
        {
        }

        public SiardArchiveReaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
