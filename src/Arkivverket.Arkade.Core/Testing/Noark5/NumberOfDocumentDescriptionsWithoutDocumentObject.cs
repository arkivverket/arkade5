using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class NumberOfDocumentDescriptionsWithoutDocumentObject : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 24);

        private bool _documentObjectIsFound;
        private int _totalNumberOfMissingDocumentObjects;
        private string _currentArchivePartSystemId;
        private readonly Dictionary<string, int> _noDocumentObjectCountPerArchivePart = new Dictionary<string, int>();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.TotalResultNumber, _totalNumberOfMissingDocumentObjects.ToString()))
            };

            if (_noDocumentObjectCountPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> noDocumentObjectCount in _noDocumentObjectCountPerArchivePart)
                {
                    if (noDocumentObjectCount.Value > 0)
                    {
                        var testResult = new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOf_PerArchivePart,
                                noDocumentObjectCount.Key, noDocumentObjectCount.Value));

                        testResults.Add(testResult);
                    }
                }
            }

            return testResults;
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
                _currentArchivePartSystemId = eventArgs.Value;
                _noDocumentObjectCountPerArchivePart.Add(_currentArchivePartSystemId, 0);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentbeskrivelse"))
            {
                if (!_documentObjectIsFound)
                {
                    _totalNumberOfMissingDocumentObjects++;

                    if (_noDocumentObjectCountPerArchivePart.ContainsKey(_currentArchivePartSystemId))
                        _noDocumentObjectCountPerArchivePart[_currentArchivePartSystemId]++;
                }
                _documentObjectIsFound = false;
            }
        }
    }
}