using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlUniqueness : AddmlProcess
    {
        public const string Name = "Control_Uniqueness";

        private readonly Dictionary<FieldIndex, HashSet<string>> _valuesPerField
            = new Dictionary<FieldIndex, HashSet<string>>();

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlUniquenessDescription;
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
            foreach (KeyValuePair<FieldIndex, HashSet<string>> entry in _valuesPerField)
            {
                FieldIndex fieldIndex = entry.Key;
                HashSet<string> fieldValues = entry.Value;

                if (fieldValues != null)
                {
                    _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(fieldIndex),
                    string.Format(Messages.ControlUniquenessMessage1)));
                }
            }

            _valuesPerField.Clear();
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;

            FieldIndex fieldIndex = field.Definition.GetIndex();
            if (!_valuesPerField.ContainsKey(fieldIndex))
            {
                _valuesPerField.Add(fieldIndex, new HashSet<string>());
            }

            HashSet<string> fieldValues = _valuesPerField[fieldIndex];

            // If null, the testresult for this value has already been created
            if (fieldValues == null)
            {
                return;
            }

            // If field already contains the value, it is not unique
            if (fieldValues.Contains(value))
            {
                _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(fieldIndex),
                    string.Format(Messages.ControlUniquenessMessage2)));

                // Set to null so we do not collect more values for this field
                _valuesPerField[fieldIndex] = null;
            }
            else
            {
                fieldValues.Add(value);
            }
        }
    }
}