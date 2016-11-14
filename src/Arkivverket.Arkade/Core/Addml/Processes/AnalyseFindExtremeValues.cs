using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseFindExtremeValues : IAddmlProcess
    {
        public const string Name = "Analyse_FindExtremeValues";

        private readonly Dictionary<FieldIndex, MinAndMax> _minAndMaxLengthPerField
            = new Dictionary<FieldIndex, MinAndMax>();

        private readonly TestRun _testRun;


        public AnalyseFindExtremeValues()
        {
            _testRun = new TestRun(Name, TestType.Content);
        }

        public string GetName()
        {
            return Name;
        }

        public string GetDescription()
        {
            return Messages.AnalyseFindExtremeValuesDescription;
        }

        public void Run(FlatFile flatFile)
        {
        }

        public void Run(Record record)
        {
        }

        public TestRun GetTestRun()
        {
            return _testRun;
        }

        public void EndOfFile()
        {
            foreach (KeyValuePair<FieldIndex, MinAndMax> entry in _minAndMaxLengthPerField)
            {
                FieldIndex fieldIndex = entry.Key;
                MinAndMax minAndMaxValue = entry.Value;
                string minLengthString = minAndMaxValue.GetMin()?.ToString() ?? "<no value>";
                string maxLengthString = minAndMaxValue.GetMax()?.ToString() ?? "<no value>";

                _testRun.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(fieldIndex),
                    string.Format(Messages.AnalyseFindExtremeValuesMessage, maxLengthString, minLengthString)));
            }

            _minAndMaxLengthPerField.Clear();
        }

        public void Run(Field field)
        {
            int valueLength = field.Value.Length;

            FieldIndex fieldIndeks = field.Definition.GetFieldIndeks();
            if (!_minAndMaxLengthPerField.ContainsKey(fieldIndeks))
            {
                _minAndMaxLengthPerField.Add(fieldIndeks, new MinAndMax());
            }

            MinAndMax minAndMaxValue = _minAndMaxLengthPerField[fieldIndeks];
            minAndMaxValue.NewValue(valueLength);
        }
    }
}