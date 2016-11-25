using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using System.Linq;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlDataFormat : AddmlProcess
    {
        public const string Name = "Control_DataFormat";

        private readonly Dictionary<FieldIndex, HashSet<string>> _incorrectDataFormat
            = new Dictionary<FieldIndex, HashSet<string>>();

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlDataFormatDescription;
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
            foreach (KeyValuePair<FieldIndex, HashSet<string>> entry in _incorrectDataFormat)
            {
                FieldIndex fieldIndex = entry.Key;
                HashSet<string> incorrectDataFormat = entry.Value;

                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFieldIndex(fieldIndex),
                    string.Format(Messages.ControlDataFormatMessage, string.Join(", ", 
                    incorrectDataFormat.Select(s => "'" + s + "'").ToList()))));
            }

            _incorrectDataFormat.Clear();
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;

            DataType dataType = field.Definition.Type;

            if (dataType.IsValid(value))
            {
                return;
            }

            // value is illegal
            FieldIndex fieldIndeks = field.Definition.GetIndex();
            if (!_incorrectDataFormat.ContainsKey(fieldIndeks))
            {
                _incorrectDataFormat.Add(fieldIndeks, new HashSet<string>());
            }

            _incorrectDataFormat[fieldIndeks].Add(value);
        }
    }
}