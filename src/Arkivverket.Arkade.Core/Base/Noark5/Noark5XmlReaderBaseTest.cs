using Arkivverket.Arkade.Core.Testing;

namespace Arkivverket.Arkade.Core.Base.Noark5
{
    /// <summary>
    ///     Base class for Noark5 tests that reads xml elements. Provides timing of element reading operations
    /// </summary>
    public abstract class Noark5XmlReaderBaseTest : Noark5BaseTest, INoark5Test
    {
        public void OnReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            Stopwatch.Start();
            ReadStartElementEvent(sender, eventArgs);
            Stopwatch.Stop();
        }

        public void OnReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            Stopwatch.Start();
            ReadAttributeEvent(sender, eventArgs);
            Stopwatch.Stop();
        }
        
        public void OnReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            Stopwatch.Start();
            ReadEndElementEvent(sender, eventArgs);
            Stopwatch.Stop();
        }

        public void OnReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            Stopwatch.Start();
            ReadElementValueEvent(sender, eventArgs);
            Stopwatch.Stop();
        }

        protected abstract void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs);
        protected abstract void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs);
        protected abstract void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs);
        protected abstract void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs);
    }
}