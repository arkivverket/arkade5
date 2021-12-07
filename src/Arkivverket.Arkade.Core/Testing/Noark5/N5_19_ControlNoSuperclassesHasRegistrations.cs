using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_19_ControlNoSuperclassesHasRegistrations : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 19);

        private readonly Dictionary<ArchivePart, List<Class>> _superClassesWithRegistrationPerArchivePart = new();
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
            bool multipleArchiveParts = _superClassesWithRegistrationPerArchivePart.Count > 1;

            int totalNumberOfSuperClassesWithRegistration =
                _superClassesWithRegistrationPerArchivePart.Sum(a => a.Value.Count);

            var testResultSet = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfSuperClassesWithRegistration))
                }
            };

            foreach ((ArchivePart archivePart, List<Class> superClassesWithRegistration) in
                _superClassesWithRegistrationPerArchivePart)
            {
                var testResults = new List<TestResult>();

                foreach (Class superClassWithRegistration in superClassesWithRegistration)
                {
                    testResults.Add(new TestResult(ResultType.Error, new Location(
                            ArkadeConstants.ArkivuttrekkXmlFileName, superClassWithRegistration.XmlLineNumber),
                        string.Format(Noark5Messages.ControlNoSuperclassesHasRegistrationsMessage,
                            superClassWithRegistration.SystemId)));
                }

                if (multipleArchiveParts)
                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = new List<TestResult>
                        {
                            new(ResultType.Success, new Location(string.Empty), string.Format(
                                Noark5Messages.NumberOf, superClassesWithRegistration.Count))
                        }.Concat(testResults).ToList()
                    });
                else
                    testResultSet.TestsResults.AddRange(testResults);
            }
            
            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("klasse"))
            {
                if (_classes.Any())
                    _classes.Peek().HasSubclass = true;

                _classes.Push(new Class(eventArgs.LineNumber));
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
                {
                    if (_superClassesWithRegistrationPerArchivePart.ContainsKey(_currentArchivePart))
                        _superClassesWithRegistrationPerArchivePart[_currentArchivePart].Add(examinedClass);
                    else
                        _superClassesWithRegistrationPerArchivePart.Add(_currentArchivePart, new List<Class>
                        {
                            examinedClass
                        });
                }
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
            public bool HasSubclass { get; set; }
            public bool HasRegistration { get; set; }
            public int XmlLineNumber { get; }

            public Class(int xmlLineNumber)
            {
                XmlLineNumber = xmlLineNumber;
            }

            public bool IsSuperClassWithRegistration()
            {
                return HasSubclass && HasRegistration;
            }
        }

    }
}
