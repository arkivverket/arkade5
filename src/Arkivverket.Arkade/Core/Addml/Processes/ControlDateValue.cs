using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlDateValue : AddmlProcess
    {
        public const string Name = "Control_Date_Value";

        private readonly Dictionary<FieldIndex, HashSet<string>> _nonDateValues
            = new Dictionary<FieldIndex, HashSet<string>>();

        private readonly List<TestResult> _testResults = new List<TestResult>();

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
            foreach (KeyValuePair<FieldIndex, HashSet<string>> entry in _nonDateValues)
            {
                FieldIndex fieldIndex = entry.Key;
                HashSet<string> nonDateValues = entry.Value;

                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFieldIndex(fieldIndex),
                    string.Format(Messages.ControlDateValueMessage, string.Join(" ", nonDateValues))));
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
            FieldIndex fieldIndeks = field.Definition.GetIndex();
            if (!_nonDateValues.ContainsKey(fieldIndeks))
            {
                _nonDateValues.Add(fieldIndeks, new HashSet<string>());
            }

            _nonDateValues[fieldIndeks].Add(value);
        }
    }
}