using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public abstract class CountElementsWithUniqueName : INoark5Test
    {
        private readonly string _elementName;
        private int _counter;

        protected CountElementsWithUniqueName(string elementName)
        {
            _elementName = elementName;
        }

        public abstract string GetName();

        public abstract TestRun GetTestRun();

        public void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
            if (e.NameEquals(_elementName))
            {
                _counter++;
            }
        }

        public void OnReadEndElementEvent(object sender, ReadElementEventArgs e)
        {
        }

        public void OnReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        public TestRun GetTestRun(string resultMessage)
        {
            var testRun = new TestRun(GetName(), TestType.ContentAnalysis);

            testRun.Add(new TestResult(ResultType.Success, new Location(""),
                string.Format(resultMessage, _counter)));

            return testRun;
        }
    }
}