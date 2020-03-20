using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_39_NumberOfCorrespondenceParts : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 39);

        private int _totalNumberOfCorrespondanceParts;
        private ArchivePart _currentArchivePart = new ArchivePart();
        private readonly Dictionary<ArchivePart, int> _correspondancePartsPerArchivePart = new Dictionary<ArchivePart, int>();
        private bool _journalPostAttributeIsFound;

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
                    string.Format(Noark5Messages.TotalResultNumber, _totalNumberOfCorrespondanceParts.ToString()))
            };

            if (_correspondancePartsPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<ArchivePart, int> correspondancePartCount in _correspondancePartsPerArchivePart)
                {
                    if (correspondancePartCount.Value > 0)
                    {
                        var testResult = new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOf_PerArchivePart, correspondancePartCount.Key.SystemId,
                                correspondancePartCount.Key.Name,
                                correspondancePartCount.Value, ""));

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
                _currentArchivePart.SystemId = eventArgs.Value;
                _correspondancePartsPerArchivePart.Add(_currentArchivePart, 0);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();

            if (eventArgs.NameEquals("korrespondansepart"))
            {
                if (_journalPostAttributeIsFound)
                {
                    _totalNumberOfCorrespondanceParts++;

                    if (_correspondancePartsPerArchivePart.Count > 0)
                    {
                        if (_correspondancePartsPerArchivePart.ContainsKey(_currentArchivePart))
                            _correspondancePartsPerArchivePart[_currentArchivePart]++;
                    }
                }

                _journalPostAttributeIsFound = false;
            }
        }

    }
}
