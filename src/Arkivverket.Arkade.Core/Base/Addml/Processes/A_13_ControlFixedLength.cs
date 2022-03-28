using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_13_ControlFixedLength : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 13);

        public const string Name = "Control_FixedLength";

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
            return Messages.ControlFixedLengthDescription;
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
            int? specifiedLength = record.Definition.RecordLength;
            if (!specifiedLength.HasValue)
            {
                return;
            }

            var actualLength = record.Value.Length;
            if (actualLength == specifiedLength)
            {
                return;
            }

            // actual record length is different than specified
            RecordIndex index = record.Definition.GetIndex();
            _testResults.Add(new TestResult(ResultType.Error,
                new Location(AddmlLocation.FromRecordIndex(index).ToString(), record.LineNumber),
                string.Format(Messages.ControlFixedLengthMessage, specifiedLength, actualLength)));
        }

        protected override void DoEndOfFile()
        {
        }

        protected override void DoRun(Field field)
        {
        }
    }
}