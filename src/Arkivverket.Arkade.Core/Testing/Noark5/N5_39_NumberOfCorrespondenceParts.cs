using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_39_NumberOfCorrespondenceParts : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 39);

        private int _totalNumberOfCorrespondenceParts;
        private ArchivePart _currentArchivePart = new();
        private readonly Dictionary<ArchivePart, int> _correspondencePartsPerArchivePart = new();

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
            bool multipleArchiveParts = _correspondencePartsPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty),
                        string.Format(Noark5Messages.TotalResultNumber, _totalNumberOfCorrespondenceParts))
                }
            };

            if (_totalNumberOfCorrespondenceParts == 0 || !multipleArchiveParts)
                return testResultSet;

            foreach ((ArchivePart archivePart, int correspondencePartsCount) in _correspondencePartsPerArchivePart)
            {
                testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(Noark5Messages.NumberOfXPerY, archivePart, correspondencePartsCount)));
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
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();

            if (eventArgs.NameEquals("korrespondansepart"))
            {
                _totalNumberOfCorrespondenceParts++;

                if (_correspondencePartsPerArchivePart.ContainsKey(_currentArchivePart))
                    _correspondencePartsPerArchivePart[_currentArchivePart]++;
                else
                    _correspondencePartsPerArchivePart.Add(_currentArchivePart, 1);
            }
        }

    }
}
