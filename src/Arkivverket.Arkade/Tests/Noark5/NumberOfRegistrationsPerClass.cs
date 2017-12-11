using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfRegistrationsPerClass : Noark5XmlReaderBaseTest
    {
        private readonly List<Class> _classes = new List<Class>();
        private string _currentArchivePartSystemId;
        private Class _currentClass;

        public override string GetName()
        {
            return Noark5Messages.NumberOfRegistrationsPerClass;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            bool multipleArchiveParts = _classes.GroupBy(j => j.ArchivePartSystemId).Count() > 1;

            foreach (Class @class in _classes.Where(c => c.NumberOfRegistrations > 0))
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfRegistrationsPerClassMessage_NumberOfRegistrations,
                        @class.SystemId, @class.NumberOfRegistrations)
                );

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, @class.ArchivePartSystemId) + " - ");

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
                _currentClass = new Class {ArchivePartSystemId = _currentArchivePartSystemId};

            if (eventArgs.Path.Matches("registrering", "klasse"))
                _currentClass.NumberOfRegistrations++;
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
            public int NumberOfRegistrations { get; set; }
        }
    }
}
