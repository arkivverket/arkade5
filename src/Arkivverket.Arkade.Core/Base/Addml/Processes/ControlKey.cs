using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using System.Linq;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class ControlKey : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 15);

        public const string Name = "Control_Key";

        private readonly Dictionary<RecordIndex, HashSet<PrimaryKeyValue>> _primaryKeyValuesPerRecord
            = new Dictionary<RecordIndex, HashSet<PrimaryKeyValue>>();

        private readonly Dictionary<RecordIndex, HashSet<PrimaryKeyValue>> _nonUniqueprimaryKeyValuesPerRecord
            = new Dictionary<RecordIndex, HashSet<PrimaryKeyValue>>();

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
                _primaryKeyValuesPerRecord.Add(recordIndex, new HashSet<PrimaryKeyValue>());
            }
            HashSet<PrimaryKeyValue> allPrimaryKeyValuesForRecord = _primaryKeyValuesPerRecord[recordIndex];

            // If null, this primary key value is already handled
            if (allPrimaryKeyValuesForRecord == null)
            {
                return;
            }

            // If list of primary key values already contains the value, it is not unique
            if (allPrimaryKeyValuesForRecord.Contains(primaryKeyValueForCurrentRecord))
            {
                if (!_nonUniqueprimaryKeyValuesPerRecord.ContainsKey(recordIndex))
                {
                    _nonUniqueprimaryKeyValuesPerRecord.Add(recordIndex, new HashSet<PrimaryKeyValue>());
                }
                _nonUniqueprimaryKeyValuesPerRecord[recordIndex].Add(primaryKeyValueForCurrentRecord);

                _primaryKeyValuesPerRecord[recordIndex].Remove(primaryKeyValueForCurrentRecord);
            }
            else
            {
                allPrimaryKeyValuesForRecord.Add(primaryKeyValueForCurrentRecord);
            }

        }

        protected override void DoEndOfFile()
        {
            foreach (KeyValuePair<RecordIndex, HashSet<PrimaryKeyValue>> entry in _nonUniqueprimaryKeyValuesPerRecord)
            {
                string text = string.Join(" ", entry.Value);
                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromRecordIndex(entry.Key),
                    string.Format(Messages.ControlKeyMessage, text)));
            }

            _nonUniqueprimaryKeyValuesPerRecord.Clear();
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