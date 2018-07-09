using Arkivverket.Arkade.Core.Tests;

namespace Arkivverket.Arkade.Core.Base.Noark5
{
    /// <summary>
    ///     Base class for Noark5 tests that reads xml elements. Provides timing of element reading operations
    /// </summary>
    public abstract class Noark5XmlReaderBaseTest : Noark5BaseTest, INoark5Test
    {
        public void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
            Stopwatch.Start();
            ReadStartElementEvent(sender, e);
            Stopwatch.Stop();
        }

        public void OnReadAttributeEvent(object sender, ReadElementEventArgs e)
        {
            Stopwatch.Start();
            ReadAttributeEvent(sender, e);
            Stopwatch.Stop();
        }
        
        public void OnReadEndElementEvent(object sender, ReadElementEventArgs e)
        {
            Stopwatch.Start();
            ReadEndElementEvent(sender, e);
            Stopwatch.Stop();
        }

        public void OnReadElementValueEvent(object sender, ReadElementEventArgs e)
        {
            Stopwatch.Start();
            ReadElementValueEvent(sender, e);
            Stopwatch.Stop();
        }

        protected abstract void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs);
        protected abstract void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs);
        protected abstract void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs);
        protected abstract void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs);
    }
}