using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_15_NumberOfEachCaseFolderStatus : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 15);

        private readonly Dictionary<ArchivePart, Dictionary<string, CaseFolderStatus>> _caseFolderStatusesPerArchivePart =
            new();
        private ArchivePart _currentArchivePart = new();

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
            bool multipleArchiveParts = _caseFolderStatusesPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();

            if (_caseFolderStatusesPerArchivePart.Count == 0)
            {
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.TotalResultNumber, 0)));
                return testResultSet;
            }

            foreach ((ArchivePart archivePart, Dictionary<string, CaseFolderStatus> numberOfEachCaseFolderStatus) in
                _caseFolderStatusesPerArchivePart)
            {
                string message = multipleArchiveParts
                    ? string.Format(Noark5Messages.NumberOf, numberOfEachCaseFolderStatus.Count)
                    : string.Format(Noark5Messages.TotalResultNumber, numberOfEachCaseFolderStatus.Count);

                var testResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), message)
                };

                foreach ((string status, CaseFolderStatus caseFolderStatus) in numberOfEachCaseFolderStatus)
                {
                    ResultType resultType = status.Equals("Avsluttet") || status.Equals("Utgår")
                        ? ResultType.Success
                        : ResultType.Error;

                    Location testResultLocation = resultType == ResultType.Success
                        ? new Location(string.Empty)
                        : new Location(ArkadeConstants.ArkivstrukturXmlFileName, caseFolderStatus.Locations);

                    testResults.Add(new TestResult(resultType, testResultLocation, string.Format(
                        Noark5Messages.NumberOfEachCaseFolderStatusMessage, status, caseFolderStatus.Count)));
                }

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                else
                    testResultSet.TestsResults = testResults;
            }

            return testResultSet;
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

            if (eventArgs.Path.Matches("saksstatus", "mappe"))
            {
                string caseFolderStatus = eventArgs.Value;
                long xmlLineNumber = eventArgs.LineNumber;
                if (_caseFolderStatusesPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_caseFolderStatusesPerArchivePart[_currentArchivePart].ContainsKey(caseFolderStatus))
                    {
                        _caseFolderStatusesPerArchivePart[_currentArchivePart][caseFolderStatus].Count++;
                        _caseFolderStatusesPerArchivePart[_currentArchivePart][caseFolderStatus].Locations.Add(xmlLineNumber);
                    }
                    else
                        _caseFolderStatusesPerArchivePart[_currentArchivePart].Add(caseFolderStatus, new CaseFolderStatus(xmlLineNumber));
                }
                else
                {
                    _caseFolderStatusesPerArchivePart.Add(_currentArchivePart, new Dictionary<string, CaseFolderStatus>
                    {
                        {caseFolderStatus, new CaseFolderStatus(xmlLineNumber)}
                    });
                }
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            // TODO: Handle non-casefolder-type subfolders?

            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        private class CaseFolderStatus
        {
            public int Count { get; set; }
            public List<long> Locations { get; }

            public CaseFolderStatus(long xmlLineNumber)
            {
                Count = 1;
                Locations = new List<long> {xmlLineNumber};
            }
        }
    }
}
