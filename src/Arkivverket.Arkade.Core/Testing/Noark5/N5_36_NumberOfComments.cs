using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_36_NumberOfComments : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 36);

        private ArchivePart _currentArchivePart = new ArchivePart();
        private readonly List<ArchivePart> _archiveParts = new List<ArchivePart>();
        private readonly Dictionary<int, string> _lastSeenElementTypeByLevel = new Dictionary<int, string>();

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
            var testResults = new List<TestResult>();
            int totalNumberOfComments = 0;

            foreach (ArchivePart archivePart in _archiveParts)
            {
                foreach (KeyValuePair<string, int> commentsForElement in archivePart.NumberOfCommentsByElement)
                {
                    string message = string.Format(
                        Noark5Messages.NumberOfCommentsMessage, commentsForElement.Key, commentsForElement.Value);

                    if (_archiveParts.Count > 1)
                        message = message.Insert(0,
                            string.Format(Noark5Messages.ArchivePartSystemId, archivePart.SystemId) + " - ");

                    testResults.Add(new TestResult(ResultType.Success, new Location(""), message));
                }

                totalNumberOfComments += archivePart.NumberOfCommentsByElement.Sum(e => e.Value);
            }

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""),
                string.Format(Noark5Messages.TotalResultNumber, totalNumberOfComments.ToString())));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("merknad"))
            {
                int parentElementLevel = eventArgs.Path.Length() - 1;

                if (_lastSeenElementTypeByLevel.TryGetValue(parentElementLevel, out string parentElementType))
                {
                    _currentArchivePart.RegisterCommentForElement(parentElementType);

                    _lastSeenElementTypeByLevel.Remove(parentElementLevel);
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
                _currentArchivePart = new ArchivePart {SystemId = eventArgs.Value};
                _archiveParts.Add(_currentArchivePart);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class ArchivePart
        {
            public string SystemId { get; set; }
            public Dictionary<string, int> NumberOfCommentsByElement { get; private set; }
                = new Dictionary<string, int>();

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