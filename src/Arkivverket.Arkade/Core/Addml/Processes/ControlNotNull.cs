using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlNotNull : AddmlProcess
    {
        public const string Name = "Control_NotNull";

        private readonly HashSet<FieldIndex> _containsNullValues
            = new HashSet<FieldIndex>();

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlNotNullDescription;
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
            foreach (FieldIndex index in _containsNullValues)
            {
                _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(index),
                    string.Format(Messages.ControlNotNullMessage)));
            }

            _containsNullValues.Clear();
        }

        protected override void DoRun(Field field)
        {
            FieldIndex index = field.Definition.GetIndex();
            if (_containsNullValues.Contains(index))
            {
                // We have already found a null value, just return
                return;
            }

            string value = field.Value;
            bool isNull = field.Definition.Type.IsNull(value);
            if (isNull)
            {
                _containsNullValues.Add(index);
            }
        }
    }
}