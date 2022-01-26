using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_17_ControlMinLength : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 17);

        public const string Name = "Control_MinLength";

        private readonly Dictionary<FieldIndex, Dictionary<long, HashSet<string>>> _valuesShorterThanMinLength = new();

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
            foreach ((FieldIndex fieldIndex, Dictionary<long, HashSet<string>> valuesMappedByRecordNumber) in _valuesShorterThanMinLength)
            {
                foreach ((long recordNumber, HashSet<string> valuesShorterThanMinLength) in valuesMappedByRecordNumber)
                    _testResults.Add(new TestResult(ResultType.Error,
                        new Location(AddmlLocation.FromFieldIndex(fieldIndex).ToString(), recordNumber),
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
            FieldIndex fieldIndex = field.Definition.GetIndex();
            if (_valuesShorterThanMinLength.ContainsKey(fieldIndex))
            {
                if (_valuesShorterThanMinLength[fieldIndex].ContainsKey(CurrentRecordNumber))
                    _valuesShorterThanMinLength[fieldIndex][CurrentRecordNumber].Add(value);
                else
                    _valuesShorterThanMinLength[fieldIndex].Add(CurrentRecordNumber, new HashSet<string> { value });
            }
            else
            {
                _valuesShorterThanMinLength.Add
                (
                    fieldIndex,
                    new Dictionary<long, HashSet<string>>
                    {
                        { CurrentRecordNumber, new HashSet<string> { value } }
                    }
                );
            }
        }
    }
}