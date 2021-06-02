using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_08_NumberOfClasses : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 8);

        private readonly Dictionary<ArchivePart, List<ClassificationSystem>> _classificationSystemsPerArchivePart = new();
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
            int totalNumberOfClasses = _classificationSystemsPerArchivePart.Sum(a =>
                a.Value.Sum(c => c.ClassesPerLevel.Values.Sum()));

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfClasses))
                }
            };

            if (totalNumberOfClasses == 0)
                return testResultSet;

            bool multipleArchiveParts = _classificationSystemsPerArchivePart.Count > 1;

            foreach ((ArchivePart archivePart, List<ClassificationSystem> classificationSystems) in
                _classificationSystemsPerArchivePart)
            {
                int numberOfClassesInArchivePart = classificationSystems.Sum(c => c.ClassesPerLevel.Values.Sum());
                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                    TestsResults = new List<TestResult>
                    {
                        new(ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.NumberOfClasses, numberOfClassesInArchivePart))
                    }
                };

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(archivePartResultSet);

                if (numberOfClassesInArchivePart == 0)
                    continue;

                foreach (ClassificationSystem classificationSystem in classificationSystems)
                {
                    var testResults = new List<TestResult>();

                    if (numberOfClassesInArchivePart > 1)
                        testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                            Noark5Messages.NumberOfClasses, classificationSystem.ClassesPerLevel.Values.Sum())));

                    foreach ((int level, int numberOfClassesAtLevel) in classificationSystem.ClassesPerLevel)
                        testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOfClassesPerLevel, level, numberOfClassesAtLevel)));

                    var classificationResultSet = new TestResultSet
                    {
                        Name = string.Format(Noark5Messages.ClassificationSystemTypeAndId,
                            classificationSystem.ClassificationGrade, classificationSystem.SystemId),
                        TestsResults = testResults,
                    };

                    if (multipleArchiveParts)
                        archivePartResultSet.TestResultSets.Add(classificationResultSet);
                    else
                        testResultSet.TestResultSets.Add(classificationResultSet);
                }
            }

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klassifikasjonssystem"))
                _currentClassificationSystem = new ClassificationSystem(eventArgs.Value);

            if (_currentClassificationSystem == null)
                return;

            if (eventArgs.NameEquals("registrering") || eventArgs.NameEquals("mappe"))
                _currentClassificationSystem.ClassificationGrade = Noark5Messages.Primary;

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
                _currentClassificationSystem = new ClassificationSystem(eventArgs.Value);

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
                if (_classificationSystemsPerArchivePart.ContainsKey(_currentArchivePart))
                    _classificationSystemsPerArchivePart[_currentArchivePart].Add(_currentClassificationSystem);
                else
                    _classificationSystemsPerArchivePart.Add(
                        _currentArchivePart,
                        new List<ClassificationSystem> {_currentClassificationSystem}
                    );
            }
            if (eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new ArchivePart();
            }
        }

        private class ClassificationSystem
        {
            public string SystemId { get; }
            public Dictionary<int, int> ClassesPerLevel { get; }
            public string ClassificationGrade { get; set; }

            public ClassificationSystem(string systemId)
            {
                SystemId = systemId;
                ClassesPerLevel = new Dictionary<int, int>();
            }
        }
    }
}
