using System.Collections.Generic;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_32_ControlBirthNumber : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 32);

        public const string Name = "Control_Birthno";

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override TestId GetId()
        {
            return _id;
        }

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
                    new Location(AddmlLocation.FromFieldIndex(field.Definition.GetIndex()).ToString(), CurrentRecordNumber),
                    string.Format(Messages.ControlBirthNumberMessage, value)));
            }
        }
    }
}