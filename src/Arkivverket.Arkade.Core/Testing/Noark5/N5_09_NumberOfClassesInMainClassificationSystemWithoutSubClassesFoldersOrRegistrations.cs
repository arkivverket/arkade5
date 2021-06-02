using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations
        : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 9);

        private readonly Stack<Class> _classes = new();

        private readonly Dictionary<ArchivePart, List<ClassificationSystem>>
            _primaryClassificationSystemsPerArchivePart = new();

        private ArchivePart _currentArchivePart = new();
        private ClassificationSystem _currentClassificationSystem;

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
            int totalNumberOfEmptyClassesInMainClassificationSystem =
                _primaryClassificationSystemsPerArchivePart.Sum(a => a.Value.Sum(c => c.NumberOfEmptyClasses));
            bool multipleArchiveParts = _primaryClassificationSystemsPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, Location.Archive, string.Format(Noark5Messages.TotalResultNumber,
                        totalNumberOfEmptyClassesInMainClassificationSystem))
                }
            };

            if (totalNumberOfEmptyClassesInMainClassificationSystem == 0)
                return testResultSet;

            foreach ((ArchivePart archivePart, List<ClassificationSystem> primaryClassificationSystems) in
                _primaryClassificationSystemsPerArchivePart)
            {
                int numberOfEmptyClassesInMainClassificationSystem =
                    primaryClassificationSystems.Sum(c => c.NumberOfEmptyClasses);

                var testResults = new List<TestResult>();

                bool multiplePrimaryClassificationSystems = primaryClassificationSystems.Count > 1;

                if (multiplePrimaryClassificationSystems)
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOf, numberOfEmptyClassesInMainClassificationSystem)));

                foreach (ClassificationSystem classificationSystem in primaryClassificationSystems)
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                        string.Format(Noark5Messages.NumberOfEmptyClassesInMainClassificationSystem,
                            classificationSystem.ClassificationSystemId, classificationSystem.NumberOfEmptyClasses)));
                }

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                else if (multiplePrimaryClassificationSystems)
                    testResultSet.TestsResults = testResults;

            }

            return testResultSet;
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
                _currentClassificationSystem = new ClassificationSystem(eventArgs.Value);

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
            {
                if (_primaryClassificationSystemsPerArchivePart.ContainsKey(_currentArchivePart))
                    _primaryClassificationSystemsPerArchivePart[_currentArchivePart].Add(_currentClassificationSystem);
                else
                    _primaryClassificationSystemsPerArchivePart.Add(_currentArchivePart, new List<ClassificationSystem>
                    {
                        _currentClassificationSystem
                    });
            }
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
            public string ClassificationSystemId { get; }
            public int NumberOfEmptyClasses { get; set; }
            public bool IsPrimary { get; set; }

            public ClassificationSystem(string classificationSystemId)
            {
                ClassificationSystemId = classificationSystemId;
            }
        }

    }
}
