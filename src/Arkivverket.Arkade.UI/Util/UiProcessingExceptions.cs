using System;

namespace Arkivverket.Arkade.UI.Util
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
