using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations
        : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 9);

        private readonly Stack<Class> _classes = new Stack<Class>();
        private ArchivePart _currentArchivePart = new ArchivePart();
        private ClassificationSystem _currentClassificationSystem;
        private readonly List<ClassificationSystem> _primaryClassificationSystems = new List<ClassificationSystem>();

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
            int numberOfEmptyClassesCount = 0;

            if (_primaryClassificationSystems.Count == 1)
            {
                testResults.Add(new TestResult(ResultType.Success, Location.Archive,
                    string.Format(Noark5Messages.TotalResultNumber,
                        _primaryClassificationSystems[0].NumberOfEmptyClasses.ToString())));
            }
            else
            {
                foreach (ClassificationSystem classificationSystem in _primaryClassificationSystems)
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfEmptyClassesInMainClassificationSystem,
                        classificationSystem.Archivepart.SystemId,
                        classificationSystem.Archivepart.Name,
                        classificationSystem.ClassificationSystemId,
                        classificationSystem.NumberOfEmptyClasses)));

                    numberOfEmptyClassesCount += classificationSystem.NumberOfEmptyClasses;
                }

                testResults.Insert(0, new TestResult(ResultType.Success, new Location(""),
                    string.Format(Noark5Messages.TotalResultNumber,
                        numberOfEmptyClassesCount.ToString())));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (_currentClassificationSystem != null && IsClassFolderOrRegistration(eventArgs))
            {
                if (_classes.Any())
                    _classes.Peek().IsCountable = false;

                if (eventArgs.NameEquals("klasse"))
                    _classes.Push(new Class {IsCountable = true});

                else // Is folder or registration
                    _currentClassificationSystem.IsPrimary = true;
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "klassifikasjonssystem"))
                _currentClassificationSystem = new ClassificationSystem(_currentArchivePart, eventArgs.Value);

            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse"))
            {
                Class currentClass = _classes.Pop();

                if (currentClass.IsCountable)
                    _currentClassificationSystem.NumberOfEmptyClasses++;
            }

            if (eventArgs.NameEquals("klassifikasjonssystem") && _currentClassificationSystem.IsPrimary && _currentClassificationSystem.NumberOfEmptyClasses > 0)
                _primaryClassificationSystems.Add(_currentClassificationSystem);
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
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

        private class ClassificationSystem
        {
            public ArchivePart Archivepart { get; }
            public string ClassificationSystemId { get; }
            public int NumberOfEmptyClasses { get; set; }
            public bool IsPrimary { get; set; }

            public ClassificationSystem(ArchivePart archivePart, string classificationSystemId)
            {
                Archivepart = archivePart;
                ClassificationSystemId = classificationSystemId;
            }
        }

    }
}
