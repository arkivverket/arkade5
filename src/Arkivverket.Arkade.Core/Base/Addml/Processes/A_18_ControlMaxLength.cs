using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_18_ControlMaxLength : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 18);

        public const string Name = "Control_MaxLength";

        private readonly Dictionary<FieldIndex, Dictionary<long, HashSet<string>>> _valuesLongerThanMaxLength = new();

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
            return Messages.ControlMaxLengthDescription;
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
            foreach ((FieldIndex fieldIndex, Dictionary<long, HashSet<string>> valuesMappedByRecordNumber) in _valuesLongerThanMaxLength)
            {
                foreach ((long recordNumber, HashSet<string> valuesLongerThanMaxLength) in valuesMappedByRecordNumber)
                {
                    _testResults.Add(new TestResult(ResultType.Error,
                        new Location(AddmlLocation.FromFieldIndex(fieldIndex).ToString(), recordNumber),
                        string.Format(Messages.ControlMaxLengthMessage, string.Join(" ", valuesLongerThanMaxLength))));
                }
            }

            _valuesLongerThanMaxLength.Clear();
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;
            int? maxLength = field.Definition.MaxLength;

            if (!maxLength.HasValue)
            {
                return;
            }

            if (value.Length <= maxLength)
            {
                return;
            }

            // value is longer than max length
            FieldIndex fieldIndex = field.Definition.GetIndex();
            if (_valuesLongerThanMaxLength.ContainsKey(fieldIndex))
            {
                if (_valuesLongerThanMaxLength[fieldIndex].ContainsKey(CurrentRecordNumber))
                    _valuesLongerThanMaxLength[fieldIndex][CurrentRecordNumber].Add(value);
                else
                    _valuesLongerThanMaxLength[fieldIndex].Add(CurrentRecordNumber, new HashSet<string> { value });
            }
            else
            {
                _valuesLongerThanMaxLength.Add
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