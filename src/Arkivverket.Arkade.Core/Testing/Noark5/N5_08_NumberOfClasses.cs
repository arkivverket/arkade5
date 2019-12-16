using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_08_NumberOfClasses : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 8);

        private ArchivePart _currentArchivePart = new ArchivePart();
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
            int totalNumberOfClasses = 0;


            foreach (ClassificationSystem classificationSystem in _classificationSystems)
            {
                string primaryOrSecondary =
                    classificationSystem.Primary ? Noark5Messages.Primary : Noark5Messages.Secondary;

                string message = string.Format(Noark5Messages.NumberOfClassesTestResultMessage,
                    classificationSystem.ArchivePart.SystemId,
                    classificationSystem.ArchivePart.Name,
                    primaryOrSecondary,
                    classificationSystem.ClassificationSystemId);

                testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                    string.Format(message + string.Format(Noark5Messages.NumberOfClasses,
                        classificationSystem.ClassesPerLevel.Values.Sum()))));

                foreach (KeyValuePair<int, int> classesOnLevel in classificationSystem.ClassesPerLevel)
                {
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        message +
                        string.Format(Noark5Messages.NumberOfClassesPerLevel,
                        classesOnLevel.Key,
                        classesOnLevel.Value))));
                }
                totalNumberOfClasses += classificationSystem.ClassesPerLevel.Values.Sum();
            }

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                Noark5Messages.TotalResultNumber,
                totalNumberOfClasses)));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klassifikasjonssystem"))
                _currentClassificationSystem = new ClassificationSystem(_currentArchivePart, eventArgs.Value);

            if (_currentClassificationSystem == null)
                return;

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

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
            if (eventArgs.Path.Matches("systemId", "arkivdel"))
            {
                _currentArchivePart.SystemId = eventArgs.Value;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klassifikasjonssystem"))
            {
                _classificationSystems.Add(_currentClassificationSystem);
            }
            if (eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new ArchivePart();
            }
        }

        private bool IsPrimaryClassificationSystem()
        {
            return _classificationSystems.All(c => c.ArchivePart.SystemId != _currentArchivePart.SystemId);
        }

        private class ClassificationSystem
        {
            public ArchivePart ArchivePart { get; }
            public string ClassificationSystemId { get; }
            public Dictionary<int, int> ClassesPerLevel { get; }
            public bool Primary { get; set; }

            public ClassificationSystem(ArchivePart currentArchivePart, string classificationSystemId)
            {
                ArchivePart = currentArchivePart;
                ClassificationSystemId = classificationSystemId;
                ClassesPerLevel = new Dictionary<int, int>();
            }
        }

    }
}
