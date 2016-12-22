using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #10
    /// </summary>
    public class NumberOfFoldersWithoutRegistrationsOrSubfolders : Noark5XmlReaderBaseTest
    {
        private bool _registrationIsFound;
        private bool _subfolderIsJustProcessed;
        private int _noRegistrationOrSubfolderCount;

        public override string GetName()
        {
            return Noark5Messages.NumberOfFoldersWithoutRegistrationsOrSubfolders;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            return new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(""), _noRegistrationOrSubfolderCount.ToString())
            };
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("registrering"))
                _registrationIsFound = true;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (!eventArgs.NameEquals("mappe"))
                return;

            if (!_registrationIsFound && !_subfolderIsJustProcessed)
                _noRegistrationOrSubfolderCount++;

            _registrationIsFound = false; // Reset
            
            if (eventArgs.Path.Matches("mappe"))
                _subfolderIsJustProcessed = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
