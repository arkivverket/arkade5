using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_47_SystemIdUniqueControl : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 47);

        private readonly SortedDictionary<string, int> _systemIdInstances = new();

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
            var testResultSet = new TestResultSet();

            foreach ((string systemId, int instances) in _systemIdInstances)
            {
                if (instances > 1)
                    testResultSet.TestsResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        string.Format(Noark5Messages.SystemIdUniqueControlMessage, systemId, instances)));
            }

            return testResultSet;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID"))
            {
                string systemId = eventArgs.Value;

                if (_systemIdInstances.ContainsKey(systemId))
                    _systemIdInstances[systemId]++;
                else _systemIdInstances[systemId] = 1;
            }
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
