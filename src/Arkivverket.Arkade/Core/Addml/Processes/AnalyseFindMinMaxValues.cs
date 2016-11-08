using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseFindMinMaxValues : IAddmlProcess
    {
        public const string Name = "Analyse_FindMinMaxValues";
        private readonly TestRun _testRun;

        private Dictionary<FieldIndex, MinAndMaxValue> minAndMaxValuesPerField 
            = new Dictionary<FieldIndex, MinAndMaxValue>();


        public AnalyseFindMinMaxValues()
        {
            _testRun = new TestRun(Name, TestType.Content);
        }

        public string GetName()
        {
            return Name;
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
            foreach (KeyValuePair<FieldIndex, MinAndMaxValue> entry in minAndMaxValuesPerField)
            {
                FieldIndex fieldIndex = entry.Key;
                MinAndMaxValue minAndMaxValue = entry.Value;
                string minValueString = minAndMaxValue.GetMinValue()?.ToString() ?? "<no value>";
                string maxValueString = minAndMaxValue.GetMaxValue()?.ToString() ?? "<no value>";

                _testRun.Add(new TestResult(ResultType.Success, FieldLocation.FromFieldIndex(fieldIndex), $"MinValue {minValueString}. MaxValue {maxValueString}."));
            }

            minAndMaxValuesPerField.Clear();
        }

        public void Run(Field field)
        {
            int value;
            if (!int.TryParse(field.Value, out value))
            {
                return;
            }

            FieldIndex fieldIndeks = field.Definition.GetFieldIndeks();
            if (!minAndMaxValuesPerField.ContainsKey(fieldIndeks))
            {
                minAndMaxValuesPerField.Add(fieldIndeks, new MinAndMaxValue());
            }

            MinAndMaxValue minAndMaxValue = minAndMaxValuesPerField[fieldIndeks];
            minAndMaxValue.NewValue(value);
        }

        private class MinAndMaxValue
        {
            private int? _minValue;
            private int? _maxValue;

            public MinAndMaxValue()
            {
            }

            public int? GetMinValue()
            {
                return _minValue;
            }

            public int? GetMaxValue()
            {
                return _maxValue;
            }

            public void NewValue(int value)
            {
                if (!_maxValue.HasValue || value > _maxValue)
                {
                    _maxValue = value;
                }

                if (!_minValue.HasValue || value < _minValue)
                {
                    _minValue = value;
                }
            }
        }
    }
}