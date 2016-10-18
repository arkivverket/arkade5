using System;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlDefinitionException : Exception
    {

        public AddmlDefinitionException(string message) : base(message)
        {
        }

        public AddmlDefinitionException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
