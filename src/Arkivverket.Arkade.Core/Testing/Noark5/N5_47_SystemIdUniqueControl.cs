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

        private readonly SortedDictionary<string, Instance> _systemIdInstances = new();

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

            foreach ((string systemId, Instance instances) in _systemIdInstances)
            {
                if (instances.Count > 1)
                    testResultSet.TestsResults.Add(new TestResult(ResultType.Error, new Location(
                            ArkadeConstants.ArkivuttrekkXmlFileName, instances.Locations),
                        string.Format(Noark5Messages.SystemIdUniqueControlMessage, systemId, instances.Count)));
            }

            return testResultSet;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID"))
            {
                string systemId = eventArgs.Value;
                long xmlLineNumber = eventArgs.LineNumber;

                if (_systemIdInstances.ContainsKey(systemId))
                {
                    _systemIdInstances[systemId].Count++;
                    _systemIdInstances[systemId].Locations.Add(xmlLineNumber);
                }
                else 
                    _systemIdInstances[systemId] = new Instance(xmlLineNumber);
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

        private class Instance
        {
            public int Count { get; set; }
            public List<long> Locations { get; }

            public Instance(long xmlLineNumber)
            {
                Count = 1;
                Locations = new List<long> {xmlLineNumber};
            }
        }
    }
}
