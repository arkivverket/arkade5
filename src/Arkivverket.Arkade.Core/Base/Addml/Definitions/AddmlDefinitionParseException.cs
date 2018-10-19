using System;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class AddmlDefinitionParseException : Exception
    {

        public AddmlDefinitionParseException(string message) : base(message)
        {
        }

        public AddmlDefinitionParseException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
