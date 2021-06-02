using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_20_NumberOfRegistrationsPerClass : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 20);

        private readonly Dictionary<ArchivePart, List<Class>> _classesPerArchivePart = new();
        private ArchivePart _currentArchivePart = new();
        private Class _currentClass;

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
            bool multipleArchiveParts = _classesPerArchivePart.Count > 1;

            int totalNumberOfClassesWithoutRegistrations =
                _classesPerArchivePart.Sum(a => a.Value.Count(c => c.NumberOfRegistrations == 0));

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfRegistrationsPerClassMessage_NumberOfClassesWithoutRegistrations,
                        totalNumberOfClassesWithoutRegistrations))
                }
            };

            foreach ((ArchivePart archivePart, List<Class> classes) in _classesPerArchivePart)
            {
                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                };

                List<TestResult> testResults = classes.Where(c => c.NumberOfRegistrations > 0)
                    .Select(@class => new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfXPerY, string.Format(Noark5Messages.ClassSystemId, @class.SystemId),
                        @class.NumberOfRegistrations))).ToList();

                if (multipleArchiveParts)
                    archivePartResultSet.TestsResults = testResults;
                else
                    testResultSet.TestsResults.AddRange(testResults);

                if (archivePartResultSet.GetNumberOfResults() > 0)
                    testResultSet.TestResultSets.Add(archivePartResultSet);
            }

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse"))
                _currentClass = new Class();

            if (eventArgs.Path.Matches("registrering", "klasse"))
                _currentClass.NumberOfRegistrations++;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("systemID", "klasse"))
                _currentClass.SystemId = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse") && _currentClass != null)
            {
                if (_classesPerArchivePart.ContainsKey(_currentArchivePart))
                    _classesPerArchivePart[_currentArchivePart].Add(_currentClass);
                else
                    _classesPerArchivePart.Add(_currentArchivePart, new List<Class>
                    {
                        _currentClass
                    });

                _currentClass = null;
            }
            if (eventArgs.NameEquals("arkivdel"))
            {
                _currentArchivePart = new ArchivePart();
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class Class
        {
            public string SystemId { get; set; }
            public int NumberOfRegistrations { get; set; }
        }

    }
}
