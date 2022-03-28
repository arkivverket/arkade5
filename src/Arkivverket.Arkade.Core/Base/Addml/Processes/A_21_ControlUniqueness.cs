using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_21_ControlUniqueness : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 21);

        public const string Name = "Control_Uniqueness";

        private readonly Dictionary<FieldIndex, Dictionary<string, HashSet<long>>> _valuesPerField = new();

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
            foreach ((FieldIndex fieldIndex, Dictionary<string, HashSet<long>> recordNumbersPerFieldValue) in _valuesPerField)
            {
                if (recordNumbersPerFieldValue.Values.All(v => v.Count == 1))
                {
                    _testResults.Add(new TestResult(ResultType.Success, AddmlLocation.FromFieldIndex(fieldIndex),
                        string.Format(Messages.ControlUniquenessMessage1)));
                    continue;
                }

                foreach ((string fieldValue, HashSet<long> recordNumber) in recordNumbersPerFieldValue.Where(v => v.Value.Count > 1))
                {
                    _testResults.Add(new TestResult(ResultType.Error,
                        new Location(AddmlLocation.FromFieldIndex(fieldIndex).ToString(), recordNumber),
                        string.Format(Messages.ControlUniquenessMessage2, fieldValue)));
                }
            }

            _valuesPerField.Clear();
        }

        protected override void DoRun(Field field)
        {
            string value = field.Value;

            FieldIndex fieldIndex = field.Definition.GetIndex();
            if (_valuesPerField.ContainsKey(fieldIndex))
            {
                if (_valuesPerField[fieldIndex].ContainsKey(value))
                    _valuesPerField[fieldIndex][value].Add(CurrentRecordNumber);
                else
                    _valuesPerField[fieldIndex].Add(value, new HashSet<long> { CurrentRecordNumber });
            }
            else
            {
                _valuesPerField.Add(fieldIndex, new Dictionary<string, HashSet<long>>
                {
                    {value, new HashSet<long>{CurrentRecordNumber}}
                });
            }
        }
    }
}