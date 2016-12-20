using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfDocumentDescriptionsWithoutDocumentObject : Noark5XmlReaderBaseTest
    {
        private bool _documentObjectIsFound;
        private int _documentObjectCount;

        public override string GetName()
        {
            return Noark5Messages.NumberOfDocumentDescriptionsWithoutDocumentObject;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            return new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(""), _documentObjectCount.ToString())
            };
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("dokumentobjekt"))
                _documentObjectIsFound = true;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (!eventArgs.Path.Matches("dokumentbeskrivelse"))
                return;

            if (!_documentObjectIsFound)
                _documentObjectCount++;

            _documentObjectIsFound = false; // Reset
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}