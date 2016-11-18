using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlAllFixedLength : AddmlProcess
    {
        public const string Name = "Control_AllFixedLength";

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlAllFixedLengthDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.Content;
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
            _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromRecordIndex(index),
                string.Format(Messages.ControlAllFixedLengthMessage, specifiedLength, actualLength)));
        }

        protected override void DoEndOfFile()
        {
        }

        protected override void DoRun(Field field)
        {
        }
    }
}