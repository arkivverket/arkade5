using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_36_ControlBooleanValue : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 36);

        public const string Name = "Control_Boolean_Value";

        private readonly Dictionary<FieldIndex, Dictionary<string, HashSet<long>>> _nonBooleanValues = new();

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
            return Messages.ControlBooleanValueDescription;
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
            foreach ((FieldIndex fieldIndex, Dictionary<string, HashSet<long>> recordNumbersPerNonBooleanValue) in _nonBooleanValues)
            {
                foreach ((string nonBooleanValue, HashSet<long> recordNumbers) in recordNumbersPerNonBooleanValue)
                {
                    _testResults.Add(new TestResult(ResultType.Error, 
                        new Location(AddmlLocation.FromFieldIndex(fieldIndex).ToString(), recordNumbers),
                        string.Format(Messages.ControlBooleanValueMessage, nonBooleanValue)));
                }
            }

            _nonBooleanValues.Clear();
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;

            DataType dt = field.Definition.Type;
            if (dt.GetType() != typeof(BooleanDataType))
            {
                return;
            }

            BooleanDataType dataType = (BooleanDataType) dt;
            if (dataType.IsValid(value, field.Definition.IsNullable))
            {
                return;
            }

            // value is illegal boolean value
            FieldIndex fieldIndex = field.Definition.GetIndex();
            if (_nonBooleanValues.ContainsKey(fieldIndex))
            {
                if (_nonBooleanValues[fieldIndex].ContainsKey(value))
                    _nonBooleanValues[fieldIndex][value].Add(CurrentRecordNumber);
                else
                    _nonBooleanValues[fieldIndex].Add(value, new HashSet<long>{CurrentRecordNumber});
            }
            else
            {
                _nonBooleanValues.Add(fieldIndex, new Dictionary<string, HashSet<long>>
                {
                    {value, new HashSet<long>{CurrentRecordNumber}}
                });
            }
        }
    }
}