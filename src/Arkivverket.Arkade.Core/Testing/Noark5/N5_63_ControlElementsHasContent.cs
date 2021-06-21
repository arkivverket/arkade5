using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_63_ControlElementsHasContent : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 63);

        private readonly List<TestResult> _testResults = new();
        private bool _elementHasContent;
        private string _lastStartElement;
        private string _lastSystemId;

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override TestResultSet GetTestResults()
        {
            return new() {TestsResults = _testResults};
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            _lastStartElement = eventArgs.Name;
            _elementHasContent = false;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID"))
                _lastSystemId = eventArgs.Value;

            _elementHasContent = !string.IsNullOrWhiteSpace(eventArgs.Value);
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Name.Equals(_lastStartElement) && !_elementHasContent)
            {
                _testResults.Add(new TestResult(ResultType.Error,
                    new Location(string.Format(Noark5Messages.ControlElementHasContent_AfterSystemId, _lastSystemId)),
                    string.Format(Noark5Messages.ControlElementHasContent_ElementHasNoContent, eventArgs.Name)
                ));
            }
        }
    }
}
