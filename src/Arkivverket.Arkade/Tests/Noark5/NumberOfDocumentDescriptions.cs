using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfDocumentDescriptions : Noark5XmlReaderBaseTest
    {
        private int _documentDescriptionCount;

        public override string GetName()
        {
            return Noark5Messages.NumberOfDocumentDescriptions;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            return new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(""), _documentDescriptionCount.ToString())
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
            if (eventArgs.Path.Matches("dokumentbeskrivelse"))
                _documentDescriptionCount++;
        }
    }
}
