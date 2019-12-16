using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_26_NumberOfDocumentObjects : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 26);

        private ArchivePart _currentArchivePart = new ArchivePart();
        private int _totalNumberOfDocumentObjects;
        private readonly Dictionary<ArchivePart, int> _documentObjectsPerArchivePart = new Dictionary<ArchivePart, int>();

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
                    string.Format(Noark5Messages.TotalResultNumber, _totalNumberOfDocumentObjects.ToString()))
            };

            if (_documentObjectsPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<ArchivePart, int> documentObjectsCount in _documentObjectsPerArchivePart)
                {
                    if (documentObjectsCount.Value > 0)
                    {
                        var testresult = new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOf_PerArchivePart, documentObjectsCount.Key.SystemId, documentObjectsCount.Key.Name,
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
                _currentArchivePart.SystemId = eventArgs.Value;
                _documentObjectsPerArchivePart.Add(_currentArchivePart, 0);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentobjekt"))
            {
                _totalNumberOfDocumentObjects++;

                if (_documentObjectsPerArchivePart.Count > 0)
                {
                    if (_documentObjectsPerArchivePart.ContainsKey(_currentArchivePart))
                        _documentObjectsPerArchivePart[_currentArchivePart]++;
                }
            }

            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

    }
}