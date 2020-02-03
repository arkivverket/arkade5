using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;

namespace Arkivverket.Arkade.Core.Testing
{
    public interface INoark5Test : IArkadeTest
    {
        void OnReadStartElementEvent(object sender, ReadElementEventArgs eventArgs);
        void OnReadAttributeEvent(object sender, ReadElementEventArgs eventArgs);
        void OnReadElementValueEvent(object sender, ReadElementEventArgs eventArgs);
        void OnReadEndElementEvent(object sender, ReadElementEventArgs eventArgs);
    }
}
