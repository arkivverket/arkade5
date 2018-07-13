using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class NumberOfDocumentObjects : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 26);

        private string _currentArchivePartSystemId;
        private int _totalNumberOfDocumentObjects;
        private readonly Dictionary<string, int> _documentObjectsPerArchivePart = new Dictionary<string, int>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfDocumentObjects;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(string.Empty), _totalNumberOfDocumentObjects.ToString())
            };

            if (_documentObjectsPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> documentObjectsCount in _documentObjectsPerArchivePart)
                {
                    if (documentObjectsCount.Value > 0)
                    {
                        var testresult = new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOf_PerArchivePart, documentObjectsCount.Key,
                                documentObjectsCount.Value));

                        testResults.Add(testresult);
                    }
                }
            }


            return testResults;
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
            {
                _currentArchivePartSystemId = eventArgs.Value;
                _documentObjectsPerArchivePart.Add(_currentArchivePartSystemId, 0);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("dokumentobjekt"))
            {
                _totalNumberOfDocumentObjects++;

                if (_documentObjectsPerArchivePart.Count > 0)
                {
                    if (_documentObjectsPerArchivePart.ContainsKey(_currentArchivePartSystemId))
                        _documentObjectsPerArchivePart[_currentArchivePartSystemId]++;
                }
            }
        }
    }
}