using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #16
    /// </summary>
    public class NumberOfRegistrationsWithoutDocumentDescription : Noark5XmlReaderBaseTest
    {
        private bool _documentDescriptionIsFound;
        private int _noDocumentDescriptionCount;

        public override string GetName()
        {
            return Noark5Messages.NumberOfRegistrationsWithoutDocumentDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            return new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(""), _noDocumentDescriptionCount.ToString())
            };
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("dokumentbeskrivelse"))
                _documentDescriptionIsFound = true;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (!eventArgs.Path.Matches("registrering"))
                return;

            if (!_documentDescriptionIsFound)
                _noDocumentDescriptionCount++;

            _documentDescriptionIsFound = false; // Reset
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
