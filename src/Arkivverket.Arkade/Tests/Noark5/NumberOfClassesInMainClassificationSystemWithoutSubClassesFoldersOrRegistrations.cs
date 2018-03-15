using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations
        : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 9);

        private readonly Stack<Class> _classes = new Stack<Class>();
        private readonly Dictionary<string, int> _emptyClassPerArchivePart = new Dictionary<string, int>();
        private bool _hasPassedMainClassificationSystemForArchivePart;

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            if (_emptyClassPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> emptyClassForArchivePart in _emptyClassPerArchivePart)
                {
                    string message = string.Format(
                        Noark5Messages.NumberOf_PerArchivePart,
                        emptyClassForArchivePart.Key,
                        emptyClassForArchivePart.Value
                    );

                    testResults.Add(new TestResult(ResultType.Success, Location.Archive, message));
                }
            }
            else
            {
                testResults.Add(new TestResult(ResultType.Success, Location.Archive,
                    _emptyClassPerArchivePart.First().Value.ToString()));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (_hasPassedMainClassificationSystemForArchivePart)
                return;

            if (IsClassFolderOrRegistration(eventArgs) && _classes.Any())
                _classes.Peek().IsCountable = false;

            if (eventArgs.NameEquals("klasse"))
                _classes.Push(new Class {IsCountable = true});
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _emptyClassPerArchivePart.Add(eventArgs.Value, 0);

                _hasPassedMainClassificationSystemForArchivePart = false;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (_hasPassedMainClassificationSystemForArchivePart)
                return;

            if (eventArgs.NameEquals("klasse"))
            {
                Class currentClass = _classes.Pop();

                if (currentClass.IsCountable)
                {
                    string currentArchivePartId = _emptyClassPerArchivePart.Keys.Last();

                    _emptyClassPerArchivePart[currentArchivePartId]++;
                }
            }

            if (eventArgs.NameEquals("klassifikasjonssystem"))
                _hasPassedMainClassificationSystemForArchivePart = true;
        }

        private static bool IsClassFolderOrRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.NameEquals("klasse")
                   || eventArgs.NameEquals("mappe")
                   || eventArgs.NameEquals("registrering");
        }

        private class Class
        {
            public bool IsCountable { get; set; }
        }
    }
}
