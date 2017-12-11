using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfDocumentObjects : Noark5XmlReaderBaseTest
    {
        private string _currentArchivePartSystemId;
        private int _totalNumberOfDocumentObjects;
        private readonly Dictionary<string, int> _documentObjectsPerArchivePart = new Dictionary<string, int>();

        public override string GetName()
        {
            return Noark5Messages.NumberOfDocumentObjects;
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
                            string.Format(Noark5Messages.NumberOfDocumentObjectsMessage, documentObjectsCount.Key,
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