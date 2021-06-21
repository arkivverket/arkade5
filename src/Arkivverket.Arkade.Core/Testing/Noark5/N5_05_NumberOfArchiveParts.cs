using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_05_NumberOfArchiveParts : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 5);

        private readonly Dictionary<string, int> _archivepartsPerArchive = new Dictionary<string, int>();
        private string _currentArchiveSystemId = string.Empty;


        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override TestResultSet GetTestResults()
        {
            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                    Noark5Messages.TotalResultNumber, _archivepartsPerArchive.Values.Sum()))
                }
            };

            foreach ((string systemId, int numberOfArchiveParts) in _archivepartsPerArchive)
            {
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfArchivePartsMessage_ArchivepartsInArchive, systemId,
                        numberOfArchiveParts))
                );
            }

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
            {
                if (_archivepartsPerArchive.ContainsKey(_currentArchiveSystemId))
                    _archivepartsPerArchive[_currentArchiveSystemId]++;
                else
                    _archivepartsPerArchive.Add(_currentArchiveSystemId, 1);
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkiv"))
                _currentArchiveSystemId = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkiv"))
                _currentArchiveSystemId = string.Empty;
        }
    }
}
