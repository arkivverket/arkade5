using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public abstract class CountElementsWithUniqueName : Noark5BaseTest
    {
        private readonly string _elementName;
        private int _counter;

        protected CountElementsWithUniqueName(string elementName)
        {
            _elementName = elementName;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
            if (e.NameEquals(_elementName))
            {
                _counter++;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs e)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(""), string.Format(GetResultMessage(), _counter))
            };
            return testResults;
        }

        protected abstract string GetResultMessage();
    }
}