using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseFindMinMaxValues : AddmlProcess
    {
        public const string Name = "Analyse_FindMinMaxValues";

        private readonly Dictionary<FieldIndex, MinAndMax> _minAndMaxValuesPerField
            = new Dictionary<FieldIndex, MinAndMax>();

        private readonly List<TestResult> _testResults = new List<TestResult>();


        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.AnalyseFindMinMaxValuesDescription;
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
        }

        protected override void DoEndOfFile()
        {
            foreach (KeyValuePair<FieldIndex, MinAndMax> entry in _minAndMaxValuesPerField)
            {
                FieldIndex fieldIndex = entry.Key;
                MinAndMax minAndMaxValue = entry.Value;
                string minValueString = minAndMaxValue.GetMin()?.ToString() ?? "<no value>";
                string maxValueString = minAndMaxValue.GetMax()?.ToString() ?? "<no value>";

                _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(fieldIndex),
                    string.Format(Messages.AnalyseFindMinMaxValuesMessage, minValueString, maxValueString)));
            }

            _minAndMaxValuesPerField.Clear();
        }

        protected override void DoRun(Field field)
        {
            int value;
            if (!int.TryParse(field.Value, out value))
            {
                return;
            }

            FieldIndex fieldIndeks = field.Definition.GetIndex();
            if (!_minAndMaxValuesPerField.ContainsKey(fieldIndeks))
            {
                _minAndMaxValuesPerField.Add(fieldIndeks, new MinAndMax());
            }

            MinAndMax minAndMaxValue = _minAndMaxValuesPerField[fieldIndeks];
            minAndMaxValue.NewValue(value);
        }
    }
}