using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class NumberOfFoldersPerClass : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 13);

        private readonly List<Class> _classes = new List<Class>();
        private string _currentArchivePartSystemId;
        private Class _currentClass;

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

            bool multipleArchiveParts = _classes.GroupBy(j => j.ArchivePartSystemId).Count() > 1;

            foreach (Class @class in _classes.Where(c => c.NumberOfFolders > 0))
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfFoldersPerClassMessage_NumberOfFolders,
                        @class.SystemId, @class.NumberOfFolders)
                );

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, @class.ArchivePartSystemId) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            int numberOfClassesWithoutFolders = _classes.Count(c => c.NumberOfFolders == 0);

            if (numberOfClassesWithoutFolders > 0)
                testResults.Add(new TestResult(ResultType.Success, new Location(""),
                    string.Format(
                        Noark5Messages.NumberOfFoldersPerClassMessage_NumberOfClassesWithoutFolders,
                        numberOfClassesWithoutFolders)
                ));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse"))
                _currentClass = new Class {ArchivePartSystemId = _currentArchivePartSystemId};

            if (eventArgs.Path.Matches("mappe", "klasse") && _currentClass != null)
                _currentClass.NumberOfFolders++;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("systemID", "klasse"))
                _currentClass.SystemId = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse") && _currentClass != null)
            {
                _classes.Add(_currentClass);
                _currentClass = null;
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class Class
        {
            public string SystemId { get; set; }
            public string ArchivePartSystemId { get; set; }
            public int NumberOfFolders { get; set; }
        }
    }
}
