using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfDocumentObjects : Noark5XmlReaderBaseTest
    {
        private int _documentObjectsCount;

        public override string GetName()
        {
            return Noark5Messages.NumberOfDocumentObjects;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            return new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(""), _documentObjectsCount.ToString())
            };
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {

        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("dokumentobjekt"))
                _documentObjectsCount++;
        }
    }
}
