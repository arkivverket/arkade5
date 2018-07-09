using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Tests;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class AnalyseFindExtremeValues : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 8);

        public const string Name = "Analyse_FindExtremeValues";

        private readonly Dictionary<FieldIndex, MinAndMax> _minAndMaxLengthPerField
            = new Dictionary<FieldIndex, MinAndMax>();

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
            return Messages.AnalyseFindExtremeValuesDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
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
            foreach (KeyValuePair<FieldIndex, MinAndMax> entry in _minAndMaxLengthPerField)
            {
                FieldIndex fieldIndex = entry.Key;
                MinAndMax minAndMaxValue = entry.Value;
                string minLengthString = minAndMaxValue.GetMin()?.ToString() ?? "<no value>";
                string maxLengthString = minAndMaxValue.GetMax()?.ToString() ?? "<no value>";

                _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(fieldIndex),
                    string.Format(Messages.AnalyseFindExtremeValuesMessage, maxLengthString, minLengthString)));
            }

            _minAndMaxLengthPerField.Clear();
        }

        protected override void DoRun(Field field)
        {
            int valueLength = field.Value.Length;

            FieldIndex fieldIndeks = field.Definition.GetIndex();
            if (!_minAndMaxLengthPerField.ContainsKey(fieldIndeks))
            {
                _minAndMaxLengthPerField.Add(fieldIndeks, new MinAndMax());
            }

            MinAndMax minAndMaxValue = _minAndMaxLengthPerField[fieldIndeks];
            minAndMaxValue.NewValue(valueLength);
        }
    }
}