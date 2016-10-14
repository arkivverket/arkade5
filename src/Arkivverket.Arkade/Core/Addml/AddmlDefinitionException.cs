using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml
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
