using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_19_ControlNoSuperclassesHasRegistrations : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 19);

        private ArchivePart _currentArchivePart = new ArchivePart();
        private readonly Stack<Class> _classes = new Stack<Class>();
        private readonly List<Class> _superClassesWithRegistration = new List<Class>();

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

            bool multipleArchiveParts = _superClassesWithRegistration.GroupBy(c => c.ArchivePart.SystemId).Count() > 1;

            foreach (var superClassWithRegistration in _superClassesWithRegistration)
            {
                var message = new StringBuilder(string.Format(
                    Noark5Messages.ControlNoSuperclassesHasRegistrationsMessage, superClassWithRegistration.SystemId));

                if (multipleArchiveParts)
                    message.Insert(0, string.Format(Noark5Messages.ArchivePartSystemId,
                            superClassWithRegistration.ArchivePart.SystemId, superClassWithRegistration.ArchivePart.Name + " - "));

                testResults.Add(new TestResult(ResultType.Error, new Location(""), message.ToString()));
            }

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""), string.Format(Noark5Messages.TotalResultNumber,
                _superClassesWithRegistration.Count)));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("klasse"))
            {
                if (_classes.Any())
                    _classes.Peek().HasSubclass = true;

                _classes.Push(new Class {ArchivePart = _currentArchivePart});
            }

            if (eventArgs.Path.Matches("registrering", "klasse"))
                _classes.Peek().HasRegistration = true;
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

                if (examinedClass.IsSuperClassWithRegistration())
                    _superClassesWithRegistration.Add(examinedClass);
            }

            if (eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class Class
        {
            public string SystemId { get; set; }
            public ArchivePart ArchivePart { get; set; }
            public bool HasSubclass { get; set; }
            public bool HasRegistration { get; set; }

            public bool IsSuperClassWithRegistration()
            {
                return HasSubclass && HasRegistration;
            }
        }

    }
}
