using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.UI
{
    public class UiPprocessingExceptions : Exception
    {

        public UiPprocessingExceptions()
        {
        }

        public UiPprocessingExceptions(string message)
            : base(message)
        {
        }
    }
}
