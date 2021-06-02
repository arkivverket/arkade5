using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 24);

        private bool _documentObjectIsFound;
        private int _totalNumberOfMissingDocumentObjects;
        private ArchivePart _currentArchivePart = new();
        private readonly Dictionary<ArchivePart, int> _noDocumentObjectCountPerArchivePart = new();

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
            bool multipleArchiveParts = _noDocumentObjectCountPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, _totalNumberOfMissingDocumentObjects))
                }
            };

            if (!multipleArchiveParts || _totalNumberOfMissingDocumentObjects == 0)
                return testResultSet;

            foreach ((ArchivePart archivePart, int noDocumentObjectCount) in _noDocumentObjectCountPerArchivePart)
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.NumberOfXPerY, archivePart, noDocumentObjectCount)));

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentobjekt"))
                _documentObjectIsFound = true;
        }


        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
                _noDocumentObjectCountPerArchivePart.Add(_currentArchivePart, 0);
            }
            
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
            {
                _currentArchivePart.Name = eventArgs.Value;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentbeskrivelse"))
            {
                if (!_documentObjectIsFound)
                {
                    _totalNumberOfMissingDocumentObjects++;

                    if (_noDocumentObjectCountPerArchivePart.ContainsKey(_currentArchivePart))
                        _noDocumentObjectCountPerArchivePart[_currentArchivePart]++;
                }
                _documentObjectIsFound = false;
            }
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

    }
}