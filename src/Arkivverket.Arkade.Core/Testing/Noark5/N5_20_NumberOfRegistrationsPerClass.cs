using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_20_NumberOfRegistrationsPerClass : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 20);

        private readonly List<Class> _classes = new List<Class>();
        private ArchivePart _currentArchivePart = new ArchivePart();
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

            bool multipleArchiveParts = _classes.GroupBy(j => j.ArchivePart.SystemId).Count() > 1;

            foreach (Class @class in _classes.Where(c => c.NumberOfRegistrations > 0))
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfRegistrationsPerClassMessage_NumberOfRegistrations,
                        @class.SystemId, @class.NumberOfRegistrations)
                );

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, @class.ArchivePart.SystemId, @class.ArchivePart.Name) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            int numberOfClassesWithoutRegistrations = _classes.Count(c => c.NumberOfRegistrations == 0);

            if (numberOfClassesWithoutRegistrations > 0)
                testResults.Add(new TestResult(ResultType.Success, new Location(""),
                    string.Format(
                        Noark5Messages.NumberOfRegistrationsPerClassMessage_NumberOfClassesWithoutRegistrations,
                        numberOfClassesWithoutRegistrations)
                ));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("klasse"))
                _currentClass = new Class {ArchivePart = _currentArchivePart};

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
                _classes.Add(_currentClass);
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
            public ArchivePart ArchivePart { get; set; }
            public int NumberOfRegistrations { get; set; }
        }

    }
}
