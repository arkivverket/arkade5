using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_12_ControlNoSuperclassesHasFolders : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 12);

        private readonly Dictionary<ArchivePart, List<Class>> _superClassesWithFolderPerArchivePart = new();
        private ArchivePart _currentArchivePart = new();
        private readonly Stack<Class> _classes = new();

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
            int totalNumberOfClassesWithFolders = _superClassesWithFolderPerArchivePart.Sum(a => a.Value.Count);
            bool multipleArchiveParts = _superClassesWithFolderPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Error, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfClassesWithFolders))
                }
            };

            if (totalNumberOfClassesWithFolders == 0)
                return testResultSet;

            foreach ((ArchivePart archivePart, List<Class> superClassesWithFolder) in
                _superClassesWithFolderPerArchivePart)
            {
                

                var testResults = new List<TestResult>();

                foreach (Class @class in superClassesWithFolder)
                {
                    testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        string.Format(Noark5Messages.ControlNoSuperclassesHasFoldersMessage, @class.SystemId)));
                }

                if (multipleArchiveParts)
                {
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = new List<TestResult>
                        {
                            new(ResultType.Error, new Location(string.Empty), string.Format(
                                Noark5Messages.NumberOf, superClassesWithFolder.Count))
                        }.Concat(testResults).ToList()
                    });
                }
                else
                {
                    testResultSet.TestsResults.AddRange(testResults);
                }
            }

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("klasse"))
            {
                if (_classes.Any())
                    _classes.Peek().HasSubclass = true;

                _classes.Push(new Class());
            }

            if (eventArgs.Path.Matches("mappe", "klasse"))
                _classes.Peek().HasFolder = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("systemID", "klasse"))
                _classes.Peek().SystemId = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse"))
            {
                Class examinedClass = _classes.Pop();

                if (examinedClass.IsSuperClassWithFolder())
                {
                    if (_superClassesWithFolderPerArchivePart.ContainsKey(_currentArchivePart))
                        _superClassesWithFolderPerArchivePart[_currentArchivePart].Add(examinedClass);
                    else
                        _superClassesWithFolderPerArchivePart.Add(_currentArchivePart, new List<Class>
                        {
                            examinedClass
                        });
                }

            }

            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart(); // Reset
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class Class
        {
            public string SystemId { get; set; }
            public bool HasSubclass { get; set; }
            public bool HasFolder { get; set; }

            public bool IsSuperClassWithFolder()
            {
                return HasSubclass && HasFolder;
            }
        }
    }
}
