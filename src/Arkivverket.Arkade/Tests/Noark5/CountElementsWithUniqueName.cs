using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    /// This is a helper class for all the analysis tests where we only are interested in how many elements with a specific name exists in the archive.
    /// Note that the only thing that is checked is the name of the element. The element name must be unique for the test to be correct.
    /// </summary>
    public abstract class CountElementsWithUniqueName : Noark5XmlReaderBaseTest
    {
        private readonly string _elementName;
        private int _counter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName">local name of the xml element (without namespace)</param>
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

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs e)
        {
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