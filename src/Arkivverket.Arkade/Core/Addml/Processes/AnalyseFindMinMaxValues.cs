using System;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class AnalyseFindMinMaxValues : IAddmlProcess
    {
        public const string Name = "Analyse_FindMinMaxValues";
        private readonly TestRun _testRun;
        private int? _minValue;
        private int? _maxValue;

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
            string minValueString = _minValue?.ToString() ?? "<no value>";
            string maxValueString = _maxValue?.ToString() ?? "<no value>";

            _testRun.Add(new TestResult(ResultType.Success,
                $"MinValue ({minValueString}). MaxValue {maxValueString}."));
        }

        public void Run(Field field)
        {
            // TODO: Use type to decide wether field is int?
            // field.Definition.GetType()

            int value;

            if (!int.TryParse(field.Value, out value))
                return;

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