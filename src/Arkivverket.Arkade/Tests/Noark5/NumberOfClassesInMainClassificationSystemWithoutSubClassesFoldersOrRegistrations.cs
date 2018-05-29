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
        private string _currentArchivePart;
        private ClassificationSystem _currentClassificationSystem;
        private readonly List<ClassificationSystem> _classificationSystems = new List<ClassificationSystem>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            if (_classificationSystems.Count == 1)
            {
                testResults.Add(new TestResult(ResultType.Success, Location.Archive,
                    _currentClassificationSystem.EmptyClasses.ToString()));
            }
            else
            {
                foreach (ClassificationSystem classificationSystem in _classificationSystems)
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfEmptyClassesInMainClassificationSystem,
                        classificationSystem.ArchivepartSystemId,
                        classificationSystem.ClassificationSystemId,
                        classificationSystem.EmptyClasses)));
                }
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klassifikasjonssystem"))
                _currentClassificationSystem = new ClassificationSystem(_currentArchivePart, eventArgs.Value);

            if (IsMainClassification(eventArgs))
                _currentClassificationSystem.Primary = true;

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
            if (eventArgs.Path.Matches("systemID", "klassifikasjonssystem"))
                _currentClassificationSystem = new ClassificationSystem(_currentArchivePart, eventArgs.Value);

            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse") && _currentClassificationSystem.Primary)
            {
                Class currentClass = _classes.Pop();

                if (currentClass.IsCountable)
                    _currentClassificationSystem.EmptyClasses++;
            }

            if (eventArgs.NameEquals("klassifikasjonssystem"))
                _classificationSystems.Add(_currentClassificationSystem);
        }
        
        private static bool IsClassFolderOrRegistration(ReadElementEventArgs eventArgs)
        {
            return eventArgs.NameEquals("klasse")
                   || eventArgs.NameEquals("mappe")
                   || eventArgs.NameEquals("registrering");
        }

        private static bool IsMainClassification(ReadElementEventArgs eventArgs)
        {
            return eventArgs.NameEquals("mappe") || eventArgs.NameEquals("registrering");
        }

        private class Class
        {
            public bool IsCountable { get; set; }
        }

        private class ClassificationSystem
        {
            public string ArchivepartSystemId { get; }
            public string ClassificationSystemId { get; }
            public int EmptyClasses { get; set; }
            public bool Primary { get; set; }

            public ClassificationSystem(string archivepartSystemId, string classificationSystemId)
            {
                ArchivepartSystemId = archivepartSystemId;
                ClassificationSystemId = classificationSystemId;
            }
        }
    }
}
