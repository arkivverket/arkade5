using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_20_ControlNotNull : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 20);

        public const string Name = "Control_NotNull";

        private readonly HashSet<FieldIndex> _containsNullValues
            = new HashSet<FieldIndex>();

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
                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFieldIndex(index),
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