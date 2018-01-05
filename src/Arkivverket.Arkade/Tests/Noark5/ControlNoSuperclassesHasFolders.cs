using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class ControlNoSuperclassesHasFolders : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 0); // TODO: Assign correct test number

        private string _currentArchivePartSystemId;
        private readonly Stack<Class> _classes = new Stack<Class>();
        private readonly List<Class> _superClassesWithFolder = new List<Class>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5Messages.ControlNoSuperclassesHasFolders;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            bool multipleArchiveParts = _superClassesWithFolder.GroupBy(c => c.ArchivePartSystemId).Count() > 1;

            foreach (var superClassWithFolder in _superClassesWithFolder)
            {
                var message = new StringBuilder(string.Format(
                    Noark5Messages.ControlNoSuperclassesHasFoldersMessage, superClassWithFolder.SystemId));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(
                            Noark5Messages.ArchivePartSystemId, superClassWithFolder.ArchivePartSystemId) + " - ");

                testResults.Add(new TestResult(ResultType.Error, new Location(""), message.ToString()));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("klasse"))
            {
                if (_classes.Any())
                    _classes.Peek().HasSubclass = true;

                _classes.Push(new Class {ArchivePartSystemId = _currentArchivePartSystemId});
            }

            if (eventArgs.Path.Matches("mappe", "klasse"))
                _classes.Peek().HasFolder = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("systemID", "klasse"))
                _classes.Peek().SystemId = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse"))
            {
                Class examinedClass = _classes.Pop();

                if (examinedClass.IsSuperClassWithFolder())
                    _superClassesWithFolder.Add(examinedClass);
            }

            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePartSystemId = null; // Reset
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class Class
        {
            public string SystemId { get; set; }
            public string ArchivePartSystemId { get; set; }
            public bool HasSubclass { get; set; }
            public bool HasFolder { get; set; }

            public bool IsSuperClassWithFolder()
            {
                return HasSubclass && HasFolder;
            }
        }
    }
}
