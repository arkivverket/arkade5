using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_13_NumberOfFoldersPerClass : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 13);

        private ArchivePart _currentArchivePart = new();
        private readonly Dictionary<ArchivePart, List<Class>> _classesPerArchivePart = new();
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
            var testResultSet = new TestResultSet();

            bool multipleArchiveParts = _classesPerArchivePart.Count > 1;

            var totalNumberOfClassesWithoutFolders = 0;

            foreach ((ArchivePart archivePart, List<Class> classes) in _classesPerArchivePart)
            {
                int numberOfClassesWithoutFolders = classes.Count(c => c.NumberOfFolders == 0);

                var archivePartResultSet = new TestResultSet
                {
                    Name = archivePart.ToString(),
                };

                List<TestResult> testResults = classes.Where(c => c.NumberOfFolders > 0)
                    .Select(@class => new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOfFoldersPerClassMessage_NumberOfFolders, @class.SystemId,
                        @class.NumberOfFolders)))
                    .ToList();

                if (numberOfClassesWithoutFolders > 0 && multipleArchiveParts)
                    testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                        string.Format(Noark5Messages.NumberOfFoldersPerClassMessage_NumberOfClassesWithoutFolders,
                            numberOfClassesWithoutFolders)));

                if (multipleArchiveParts)
                {
                    archivePartResultSet.TestsResults = testResults;
                    testResultSet.TestResultSets.Add(archivePartResultSet);
                }
                else
                {
                    testResultSet.TestsResults = testResults;
                }

                totalNumberOfClassesWithoutFolders += numberOfClassesWithoutFolders;
            }

            testResultSet.TestsResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                string.Format(Noark5Messages.NumberOfFoldersPerClassMessage_NumberOfClassesWithoutFolders,
                    totalNumberOfClassesWithoutFolders)));

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse"))
                _currentClass = new Class();

            if (eventArgs.Path.Matches("mappe", "klasse") && _currentClass != null)
                _currentClass.NumberOfFolders++;
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
                    _classesPerArchivePart.Add(_currentArchivePart, new List<Class> {_currentClass});

                _currentClass = null;
            }

            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class Class
        {
            public string SystemId { get; set; }
            public int NumberOfFolders { get; set; }
        }

    }
}
