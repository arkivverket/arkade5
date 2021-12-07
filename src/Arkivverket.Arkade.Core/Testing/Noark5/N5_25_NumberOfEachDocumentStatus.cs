using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_25_NumberOfEachDocumentStatus : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 25);

        private readonly Dictionary<ArchivePart, Dictionary<string, DocumentStatus>> _documentStatusesPerArchivePart =
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
            bool multipleArchiveParts = _documentStatusesPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();

            foreach ((ArchivePart archivePart, Dictionary<string, DocumentStatus> documentStatuses) in _documentStatusesPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach ((string status, DocumentStatus documentStatus) in documentStatuses)
                {
                    ResultType resultType = status.Equals("Dokumentet er ferdigstilt")
                        ? ResultType.Success
                        : ResultType.Error;

                    Location testResultLocation = resultType == ResultType.Success
                        ? new Location(string.Empty)
                        : new Location(ArkadeConstants.ArkivuttrekkXmlFileName, documentStatus.Locations);

                    testResults.Add(new TestResult(resultType, testResultLocation,
                        string.Format(Noark5Messages.NumberOfEachDocumentStatusMessage, status, documentStatus.Count)));
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

            if (eventArgs.Path.Matches("dokumentstatus", "dokumentbeskrivelse"))
            {
                string documentStatus = eventArgs.Value;
                int xmlLineNumber = eventArgs.LineNumber;

                if (_documentStatusesPerArchivePart.ContainsKey(_currentArchivePart))
                {
                    if (_documentStatusesPerArchivePart[_currentArchivePart].ContainsKey(documentStatus))
                    {
                        _documentStatusesPerArchivePart[_currentArchivePart][documentStatus].Count++;
                        _documentStatusesPerArchivePart[_currentArchivePart][documentStatus].Locations.Add(xmlLineNumber);
                    }
                    else
                        _documentStatusesPerArchivePart[_currentArchivePart].Add(documentStatus, new DocumentStatus(xmlLineNumber));
                }
                else
                {
                    _documentStatusesPerArchivePart.Add(_currentArchivePart, new Dictionary<string, DocumentStatus>
                    {
                        {documentStatus, new DocumentStatus(xmlLineNumber)}
                    });
                }
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if(eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new ArchivePart();
            }
        }

        private class DocumentStatus
        {
            public int Count { get; set; }
            public List<int> Locations { get; }

            public DocumentStatus(int xmlLineNumber)
            {
                Count = 1;
                Locations = new List<int> { xmlLineNumber };
            }
        }
    }
}
