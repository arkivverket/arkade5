using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlMinLength : AddmlProcess
    {
        public const string Name = "Control_MinLength";

        private readonly Dictionary<FieldIndex, HashSet<string>> _valuesShorterThanMinLength
            = new Dictionary<FieldIndex, HashSet<string>>();

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlMinLengthDescription;
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
            foreach (KeyValuePair<FieldIndex, HashSet<string>> entry in _valuesShorterThanMinLength)
            {
                FieldIndex fieldIndex = entry.Key;
                HashSet<string> valuesShorterThanMinLength = entry.Value;

                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFieldIndex(fieldIndex),
                    string.Format(Messages.ControlMinLengthMessage, string.Join(" ", valuesShorterThanMinLength))));
            }

            _valuesShorterThanMinLength.Clear();
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;
            int? minLength = field.Definition.MinLength;

            if (!minLength.HasValue)
            {
                return;
            }

            if (value.Length >= minLength)
            {
                return;
            }

            // value is shorter than min length
            FieldIndex fieldIndeks = field.Definition.GetIndex();
            if (!_valuesShorterThanMinLength.ContainsKey(fieldIndeks))
            {
                _valuesShorterThanMinLength.Add(fieldIndeks, new HashSet<string>());
            }

            _valuesShorterThanMinLength[fieldIndeks].Add(value);
        }
    }
}