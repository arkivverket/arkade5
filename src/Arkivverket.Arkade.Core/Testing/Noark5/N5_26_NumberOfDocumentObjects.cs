using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_26_NumberOfDocumentObjects : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 26);

        private ArchivePart _currentArchivePart = new();
        private int _totalNumberOfDocumentObjects;
        private readonly Dictionary<ArchivePart, int> _documentObjectsPerArchivePart = new();

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
            bool multipleArchiveParts = _documentObjectsPerArchivePart.Count > 1;

            var testResultSets = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _totalNumberOfDocumentObjects))
                }
            };

            if (!multipleArchiveParts)
                return testResultSets;

            foreach ((ArchivePart archivePart, int documentObjectsCount) in _documentObjectsPerArchivePart)
                testResultSets.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfXPerY, archivePart, documentObjectsCount)));

            return testResultSets;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;
            
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentobjekt"))
            {
                _totalNumberOfDocumentObjects++;

                if (_documentObjectsPerArchivePart.ContainsKey(_currentArchivePart))
                    _documentObjectsPerArchivePart[_currentArchivePart]++;
                else
                    _documentObjectsPerArchivePart.Add(_currentArchivePart, 1);
            }

            if(eventArgs.NameEquals("arkivdel"))
            {
                if (!_documentObjectsPerArchivePart.ContainsKey(_currentArchivePart))
                    _documentObjectsPerArchivePart.Add(_currentArchivePart, 0);
                _currentArchivePart = new ArchivePart();
            }
        }

    }
}