using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfCaseParts : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 35);

        private string _currentArchivePartSystemId;
        private int _totalNumberOfCaseParts;
        private readonly Dictionary<string, int> _casePartsPerArchivePart = new Dictionary<string, int>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfCaseParts;
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
                    _totalNumberOfCaseParts.ToString())
            };

            if (_casePartsPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> casePartCount in _casePartsPerArchivePart)
                {
                    if (casePartCount.Value > 0)
                    {
                        var testResult = new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOfCasePartsMessage, casePartCount.Key,
                                casePartCount.Value));

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
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePartSystemId = eventArgs.Value;
                _casePartsPerArchivePart.Add(_currentArchivePartSystemId, 0);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("sakspart"))
            {
                _totalNumberOfCaseParts++;

                if (_casePartsPerArchivePart.Count > 0)
                {
                    if (_casePartsPerArchivePart.ContainsKey(_currentArchivePartSystemId))
                        _casePartsPerArchivePart[_currentArchivePartSystemId]++;
                }
            }
        }
    }
}