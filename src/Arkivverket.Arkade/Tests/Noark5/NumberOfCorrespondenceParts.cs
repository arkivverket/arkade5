using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfCorrespondenceParts : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 39);

        private int _totalNumberOfCorrespondanceParts;
        private string _currentArchivePartSystemId;
        private readonly Dictionary<string, int> _correspondancePartsPerArchivePart = new Dictionary<string, int>();
        private bool _journalPostAttributeIsFound;

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfCorrespondenceParts;
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
                    _totalNumberOfCorrespondanceParts.ToString())
            };

            if (_correspondancePartsPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> correspondancePartCount in _correspondancePartsPerArchivePart)
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
                _correspondancePartsPerArchivePart.Add(_currentArchivePartSystemId, 0);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("korrespondansepart"))
            {
                if (_journalPostAttributeIsFound)
                {
                    _totalNumberOfCorrespondanceParts++;

                    if (_correspondancePartsPerArchivePart.Count > 0)
                    {
                        if (_correspondancePartsPerArchivePart.ContainsKey(_currentArchivePartSystemId))
                            _correspondancePartsPerArchivePart[_currentArchivePartSystemId]++;
                    }
                }

                _journalPostAttributeIsFound = false;
            }
        }
    }
}
