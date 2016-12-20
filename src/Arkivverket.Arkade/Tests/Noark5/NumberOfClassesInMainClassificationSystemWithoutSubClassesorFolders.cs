using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders : Noark5XmlReaderBaseTest
    {
        private int _classesWithoutSubClassOrFolder;
        private bool _foundSubClassOrFolder;
        private bool _isInsideClass;
        private bool _isInsideMainClassificationSystem;
        private bool _testingIsFinished;

        public override string GetName()
        {
            return Noark5Messages.NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            return new List<TestResult>
            {
                new TestResult(ResultType.Success, Location.Archive, "" + _classesWithoutSubClassOrFolder)
            };
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (_testingIsFinished)
            {
                return;
            }

            if (!_isInsideMainClassificationSystem
                && eventArgs.NameEquals("klassifikasjonssystem")
                && eventArgs.Path.Matches("klassifikasjonssystem", "arkivdel", "arkiv"))
            {
                _isInsideMainClassificationSystem = true;
            }

            if (eventArgs.NameEquals("klasse")
                && eventArgs.Path.Matches("klasse", "klassifikasjonssystem"))
            {
                _isInsideClass = true;
            }

            if ((_isInsideClass
                 && eventArgs.Path.Matches("klasse", "klasse", "klassifikasjonssystem"))
                || eventArgs.Path.Matches("mappe", "klasse", "klassifikasjonssystem"))
            {
                _foundSubClassOrFolder = true;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (_testingIsFinished)
            {
                return;
            }

            if (_isInsideClass && (eventArgs.NameEquals("klasse") & eventArgs.Path.Matches("klassifikasjonssystem")))
            {
                _isInsideClass = false;
                if (!_foundSubClassOrFolder)
                {
                    _classesWithoutSubClassOrFolder++;
                }

                _foundSubClassOrFolder = false;
            }

            if (_isInsideMainClassificationSystem && eventArgs.NameEquals("klassifikasjonssystem"))
            {
                _testingIsFinished = true;
            }
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}