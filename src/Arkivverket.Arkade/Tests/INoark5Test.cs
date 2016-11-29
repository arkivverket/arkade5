using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;

namespace Arkivverket.Arkade.Tests
{
    public interface INoark5Test
    {
        string GetName();

        TestRun GetTestRun();
        void OnReadStartElementEvent(object sender, ReadElementEventArgs e);
        void OnReadEndElementEvent(object sender, ReadElementEventArgs e);
        void OnReadElementValueEvent(object sender, ReadElementEventArgs eventArgs);
    }
}