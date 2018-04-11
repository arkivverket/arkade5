using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfFoldersWithoutRegistrationsOrSubfolders : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 14);

        private bool _registrationIsFound;
        private bool _subfolderIsJustProcessed;
        private int _noRegistrationOrSubfolderCount;
        private string _currentArchivePartSystemId;

        private readonly Dictionary<string, int> _noRegistrationOrSubfolderCountPerArchivePart =
            new Dictionary<string, int>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfFoldersWithoutRegistrationsOrSubfolders;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(""), _noRegistrationOrSubfolderCount.ToString())
            };

            if (_noRegistrationOrSubfolderCountPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> noRegistrationOrSubfolderCount in
                    _noRegistrationOrSubfolderCountPerArchivePart)
                {
                    if (noRegistrationOrSubfolderCount.Value > 0)
                    {
                        var testResult = new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOf_PerArchivePart, noRegistrationOrSubfolderCount.Key,
                                noRegistrationOrSubfolderCount.Value));

                        testResults.Add(testResult);
                    }
                }
            }
            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering"))
                _registrationIsFound = true;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePartSystemId = eventArgs.Value;
                _noRegistrationOrSubfolderCountPerArchivePart.Add(_currentArchivePartSystemId, 0);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (!eventArgs.NameEquals("mappe"))
                return;

            if (!_registrationIsFound && !_subfolderIsJustProcessed)
            {
                _noRegistrationOrSubfolderCount++;

                if (_noRegistrationOrSubfolderCountPerArchivePart.Count > 0)
                {
                    if (_noRegistrationOrSubfolderCountPerArchivePart.ContainsKey(_currentArchivePartSystemId))
                        _noRegistrationOrSubfolderCountPerArchivePart[_currentArchivePartSystemId]++;
                }
            }

            _registrationIsFound = false; // Reset

            if (eventArgs.Path.Matches("mappe"))
                _subfolderIsJustProcessed = true;
        }
    }
}
