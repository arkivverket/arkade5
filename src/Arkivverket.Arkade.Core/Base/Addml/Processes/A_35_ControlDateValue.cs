using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_35_ControlDateValue : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 35);

        public const string Name = "Control_Date_Value";

        private readonly Dictionary<FieldIndex, Dictionary<string, HashSet<long>>> _nonDateValues = new ();

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
            return Messages.ControlDateValueDescription;
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
            foreach ((FieldIndex fieldIndex, Dictionary<string, HashSet<long>> recordNumbersPerNonDateValue) in _nonDateValues)
            {
                foreach ((string nonDateValue, HashSet<long> recordNumbers) in recordNumbersPerNonDateValue)
                {
                    _testResults.Add(new TestResult(ResultType.Error,
                        new Location(AddmlLocation.FromFieldIndex(fieldIndex).ToString(), recordNumbers),
                        string.Format(Messages.ControlDateValueMessage, nonDateValue)));
                }
            }

            _nonDateValues.Clear();
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;

            DataType dt = field.Definition.Type;
            if (dt.GetType() != typeof(DateDataType))
            {
                return;
            }

            DateDataType dataType = (DateDataType) dt;
            if (dataType.IsValid(value))
            {
                return;
            }

            // value is illegal date value
            FieldIndex fieldIndex = field.Definition.GetIndex();
            if (_nonDateValues.ContainsKey(fieldIndex))
            {
                if (_nonDateValues[fieldIndex].ContainsKey(value))
                    _nonDateValues[fieldIndex][value].Add(CurrentRecordNumber);
                else
                    _nonDateValues[fieldIndex].Add(value, new HashSet<long>{CurrentRecordNumber});
            }
            else
            {
                _nonDateValues.Add(fieldIndex, new Dictionary<string, HashSet<long>>
                {
                    {value, new HashSet<long>{CurrentRecordNumber}}
                });
            }
        }
    }
}