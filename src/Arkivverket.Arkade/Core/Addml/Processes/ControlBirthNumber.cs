using System.Collections.Generic;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlBirthNumber : AddmlProcess
    {
        public const string Name = "Control_Birthno";

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlBirthNumberDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoEndOfFile()
        {
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;

            bool ok = NorwegianBirthNumber.Verify(value);
            if (!ok)
            {
                _testResults.Add(new TestResult(ResultType.Error,
                    AddmlLocation.FromFieldIndex(field.Definition.GetIndex()),
                    string.Format(Messages.ControlBirthNumberMessage, value)));
            }
        }
    }
}