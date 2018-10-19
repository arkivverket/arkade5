using System;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArkadeException : Exception
    {
        public ArkadeException(string message) : base(message)
        {
        }

        public ArkadeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}