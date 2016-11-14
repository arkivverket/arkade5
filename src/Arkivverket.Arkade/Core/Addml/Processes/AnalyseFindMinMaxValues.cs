using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseFindMinMaxValues : IAddmlProcess
    {
        public const string Name = "Analyse_FindMinMaxValues";
        public const string Description = "Finner laveste og høyeste verdi i feltet";

        private readonly TestRun _testRun;
        private readonly Dictionary<FieldIndex, MinAndMaxValue> _minAndMaxValuesPerField 
            = new Dictionary<FieldIndex, MinAndMaxValue>();


        public AnalyseFindMinMaxValues()
        {
            _testRun = new TestRun(Name, TestType.Content);
        }

        public string GetName()
        {
            return Name;
        }

        public string GetDescription()
        {
            return Description;
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
            foreach (KeyValuePair<FieldIndex, MinAndMaxValue> entry in _minAndMaxValuesPerField)
            {
                FieldIndex fieldIndex = entry.Key;
                MinAndMaxValue minAndMaxValue = entry.Value;
                string minValueString = minAndMaxValue.GetMinValue()?.ToString() ?? "<no value>";
                string maxValueString = minAndMaxValue.GetMaxValue()?.ToString() ?? "<no value>";

                _testRun.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(fieldIndex), $"MinValue {minValueString}. MaxValue {maxValueString}."));
            }

            _minAndMaxValuesPerField.Clear();
        }

        public void Run(Field field)
        {
            int value;
            if (!int.TryParse(field.Value, out value))
            {
                return;
            }

            FieldIndex fieldIndeks = field.Definition.GetFieldIndeks();
            if (!_minAndMaxValuesPerField.ContainsKey(fieldIndeks))
            {
                _minAndMaxValuesPerField.Add(fieldIndeks, new MinAndMaxValue());
            }

            MinAndMaxValue minAndMaxValue = _minAndMaxValuesPerField[fieldIndeks];
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