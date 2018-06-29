using System;

namespace Arkivverket.Arkade.GUI.Util
{
    public class GuiProcessingExceptions : Exception
    {

        public GuiProcessingExceptions()
        {
        }

        public GuiProcessingExceptions(string message)
            : base(message)
        {
        }
    }
}
