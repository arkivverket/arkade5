using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Tests.Noark5
{
    public class NumberOfDocumentFlows : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 41);

        private int _totalNumberOfDocumentFlows;
        private string _currentArchivePartSystemId;
        private bool _journalPostAttributeIsFound;
        private readonly Dictionary<string, int> _documentFlowsPerArchivePart = new Dictionary<string, int>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfDocumentFlows;
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
                    _totalNumberOfDocumentFlows.ToString())
            };

            if (_documentFlowsPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> correspondancePartCount in _documentFlowsPerArchivePart)
                {
                    if (correspondancePartCount.Value > 0)
                    {
                        var testResult = new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOf_PerArchivePart, correspondancePartCount.Key,
                                correspondancePartCount.Value));

                        testResults.Add(testResult);
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
            if (Noark5TestHelper.IdentifiesJournalPostRegistration(eventArgs))
                _journalPostAttributeIsFound = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePartSystemId = eventArgs.Value;
                _documentFlowsPerArchivePart.Add(_currentArchivePartSystemId, 0);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentflyt"))
            {
                if (_journalPostAttributeIsFound)
                {
                    _totalNumberOfDocumentFlows++;

                    if (_documentFlowsPerArchivePart.Count > 0)
                    {
                        if (_documentFlowsPerArchivePart.ContainsKey(_currentArchivePartSystemId))
                            _documentFlowsPerArchivePart[_currentArchivePartSystemId]++;
                    }
                }

                _journalPostAttributeIsFound = false;
            }
        }
    }
}
