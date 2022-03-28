using System.Collections.Generic;
using System.Collections.Immutable;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using System.Linq;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class A_15_ControlKey : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 15);

        public const string Name = "Control_Key";

        private readonly Dictionary<RecordIndex, Dictionary<PrimaryKeyValue, HashSet<long>>> _primaryKeyValuesPerRecord = new();

        private readonly Dictionary<RecordIndex, Dictionary<PrimaryKeyValue, HashSet<long>>> _nonUniquePrimaryKeyValuesPerRecord = new();

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
            return Messages.ControlKeyDescription;
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
            List<string> primaryKeyValues = new List<string>();
            foreach (Field field in record.Fields)
            {
                if (field.Definition.IsPartOfPrimaryKey())
                {
                    primaryKeyValues.Add(field.Value);
                }
            }
            PrimaryKeyValue primaryKeyValueForCurrentRecord = new PrimaryKeyValue(primaryKeyValues);

            RecordIndex recordIndex = record.Definition.GetIndex();
            if (!_primaryKeyValuesPerRecord.ContainsKey(recordIndex))
            {
                _primaryKeyValuesPerRecord.Add(recordIndex, new Dictionary<PrimaryKeyValue, HashSet<long>>());
            }

            var allPrimaryKeyValuesForRecord = _primaryKeyValuesPerRecord[recordIndex];


            // If list of primary key values already contains the value, it is not unique
            if (allPrimaryKeyValuesForRecord.ContainsKey(primaryKeyValueForCurrentRecord))
            {
                if (_nonUniquePrimaryKeyValuesPerRecord.ContainsKey(recordIndex))
                {
                    if (_nonUniquePrimaryKeyValuesPerRecord[recordIndex].ContainsKey(primaryKeyValueForCurrentRecord))
                        _nonUniquePrimaryKeyValuesPerRecord[recordIndex][primaryKeyValueForCurrentRecord].Add(record.LineNumber);
                    else
                        _nonUniquePrimaryKeyValuesPerRecord[recordIndex].Add(primaryKeyValueForCurrentRecord, 
                            new HashSet<long>(allPrimaryKeyValuesForRecord[primaryKeyValueForCurrentRecord]) {record.LineNumber});
                }
                else
                {
                    _nonUniquePrimaryKeyValuesPerRecord.Add(recordIndex, new Dictionary<PrimaryKeyValue, HashSet<long>>
                    {
                        {primaryKeyValueForCurrentRecord, new HashSet<long>(allPrimaryKeyValuesForRecord[primaryKeyValueForCurrentRecord]){record.LineNumber}}
                    });
                }
            }
            else
            {
                allPrimaryKeyValuesForRecord.Add(primaryKeyValueForCurrentRecord, new HashSet<long>{record.LineNumber});
            }

        }

        protected override void DoEndOfFile()
        {
            foreach ((RecordIndex recordIndex, Dictionary<PrimaryKeyValue, HashSet<long>> primaryKeyValues) in _nonUniquePrimaryKeyValuesPerRecord)
            {
                string text = string.Join(" ", primaryKeyValues.Keys);
                IEnumerable<long> recordNumbers = primaryKeyValues.SelectMany(p => p.Value).ToImmutableSortedSet();
                _testResults.Add(new TestResult(ResultType.Error, 
                    new Location(AddmlLocation.FromRecordIndex(recordIndex).ToString(), recordNumbers),
                    string.Format(Messages.ControlKeyMessage, text)));
            }

            _nonUniquePrimaryKeyValuesPerRecord.Clear();
            _primaryKeyValuesPerRecord.Clear();
        }

        protected override void DoRun(Field field)
        {
        }
    }

    public class PrimaryKeyValue
    {
        private List<string> _primaryKeyValues;

        public PrimaryKeyValue(List<string> primaryKeyValues)
        {
            _primaryKeyValues = primaryKeyValues;
        }


        protected bool Equals(PrimaryKeyValue other)
        {
            return _primaryKeyValues.SequenceEqual(other._primaryKeyValues);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PrimaryKeyValue)obj);
        }

        public override int GetHashCode()
        {
            int hc = 0;
            if (_primaryKeyValues != null)
            {
                foreach (var p in _primaryKeyValues)
                {
                    hc ^= p.GetHashCode();
                }
            }
            return hc;
        }

        public override string ToString()
        {
            return string.Join(",", _primaryKeyValues);
        }

    }
}