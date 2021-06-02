using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_36_NumberOfComments : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 36);

        private N5_36_ArchivePart _currentArchivePart = new();
        private readonly List<N5_36_ArchivePart> _archiveParts = new();
        private readonly Dictionary<int, string> _lastSeenElementTypeByLevel = new();

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
            bool multipleArchiveParts = _archiveParts.Count > 1;

            int totalNumberOfComments = _archiveParts.Sum(a => a.NumberOfCommentsByElement.Values.Sum());

            var testResultSets = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfComments))
                }
            };

            if (totalNumberOfComments == 0)
                return testResultSets;

            foreach (N5_36_ArchivePart archivePart in _archiveParts)
            {
                var numberOfCommentsPerArchivePart = 0;
                var testResults = new List<TestResult>();

                foreach ((string element, int numberOfComments) in archivePart.NumberOfCommentsByElement)
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                        string.Format(Noark5Messages.NumberOfCommentsMessage, element, numberOfComments)));

                    numberOfCommentsPerArchivePart += numberOfComments;
                }

                if (multipleArchiveParts)
                {
                    testResults.Insert(0, new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOf, numberOfCommentsPerArchivePart)));

                    testResultSets.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                }
                else
                    testResultSets.TestsResults.AddRange(testResults);
                
            }

            return testResultSets;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("merknad"))
            {
                int parentElementLevel = eventArgs.Path.Length() - 1;

                if (_lastSeenElementTypeByLevel.TryGetValue(parentElementLevel, out string parentElementType))
                {
                    _currentArchivePart.RegisterCommentForElement(parentElementType);
                }
                else
                {
                    _currentArchivePart.RegisterCommentForElement(eventArgs.Path.GetParent());
                }
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            int elementLevel = eventArgs.Path.Length();

            if (_lastSeenElementTypeByLevel.ContainsKey(elementLevel))
                _lastSeenElementTypeByLevel[elementLevel] = eventArgs.Value;
            else
                _lastSeenElementTypeByLevel.Add(elementLevel, eventArgs.Value);
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart = new N5_36_ArchivePart() {SystemId = eventArgs.Value};
                _archiveParts.Add(_currentArchivePart);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            int currentElementLevel = eventArgs.Path.Length();

            if (_lastSeenElementTypeByLevel.ContainsKey(currentElementLevel))
                _lastSeenElementTypeByLevel.Remove(currentElementLevel);

            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = null;
        }

        private class N5_36_ArchivePart : ArchivePart 
        {
            public Dictionary<string, int> NumberOfCommentsByElement { get; private set; } = new();

            public void RegisterCommentForElement(string element)
            {
                if (NumberOfCommentsByElement == null)
                    NumberOfCommentsByElement = new Dictionary<string, int>();

                if (NumberOfCommentsByElement.ContainsKey(element))
                    NumberOfCommentsByElement[element]++;
                else
                    NumberOfCommentsByElement.Add(element, 1);
            }
        }
    }
}