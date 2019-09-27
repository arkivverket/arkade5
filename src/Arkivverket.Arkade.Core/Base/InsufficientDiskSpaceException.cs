using System;

namespace Arkivverket.Arkade.Core.Base
{
    public class InsufficientDiskSpaceException : Exception
    {
        public InsufficientDiskSpaceException(string message) : base(message)
        {
        }

        public InsufficientDiskSpaceException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
