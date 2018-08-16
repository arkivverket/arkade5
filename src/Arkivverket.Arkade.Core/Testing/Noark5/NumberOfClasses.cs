using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class NumberOfClasses : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 8);

        private string _currentArchivePart;
        private readonly List<ClassificationSystem> _classificationSystems = new List<ClassificationSystem>();
        private ClassificationSystem _currentClassificationSystem;

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

            foreach (ClassificationSystem classificationSystem in _classificationSystems)
            {
                string primaryOrSecondary =
                    classificationSystem.Primary ? Noark5Messages.Primary : Noark5Messages.Secondary;

                testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                    Noark5Messages.NumberOfClasses,
                    classificationSystem.ArchivepartSystemId,
                    primaryOrSecondary,
                    classificationSystem.ClassificationSystemId,
                    classificationSystem.ClassesPerLevel.Values.Sum())));

                foreach (KeyValuePair<int, int> classesOnLevel in classificationSystem.ClassesPerLevel)
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfClassesPerLevel,
                        classificationSystem.ArchivepartSystemId,
                        primaryOrSecondary,
                        classificationSystem.ClassificationSystemId,
                        classesOnLevel.Key,
                        classesOnLevel.Value)));
                }
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klassifikasjonssystem"))
                _currentClassificationSystem = new ClassificationSystem(_currentArchivePart, eventArgs.Value);

            if (eventArgs.NameEquals("registrering") || eventArgs.NameEquals("mappe"))
                _currentClassificationSystem.Primary = true;
            
            if (eventArgs.NameEquals("klasse"))
            {
                int level = eventArgs.Path.GetSameElementSubLevel();

                if (_currentClassificationSystem.ClassesPerLevel.ContainsKey(level))
                    _currentClassificationSystem.ClassesPerLevel[level]++;
                else
                    _currentClassificationSystem.ClassesPerLevel.Add(level, 1);
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
                _currentArchivePart = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klassifikasjonssystem"))
            {
                _classificationSystems.Add(_currentClassificationSystem);
            }
        }

        private bool IsPrimaryClassificationSystem()
        {
            return _classificationSystems.All(c => c.ArchivepartSystemId != _currentArchivePart);
        }

        private class ClassificationSystem
        {
            public string ArchivepartSystemId { get; }
            public string ClassificationSystemId { get; }
            public Dictionary<int, int> ClassesPerLevel { get; }
            public bool Primary { get; set; }

            public ClassificationSystem(string archivepartSystemId, string classificationSystemId)
            {
                ArchivepartSystemId = archivepartSystemId;
                ClassificationSystemId = classificationSystemId;
                ClassesPerLevel = new Dictionary<int, int>();
            }
        }
    }
}
