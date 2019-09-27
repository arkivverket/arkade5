using System.Collections.Generic;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes.Internal
{
    public class AI_01_CollectPrimaryKey : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.AddmlInternal, 1);

        public const string Name = "Collect_PrimaryKey";

        public readonly Dictionary<string, HashSet<string>> _primaryKeys = new Dictionary<string, HashSet<string>>();
        
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
            return "";
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override void DoEndOfFile()
        {
        }

        protected override List<TestResult> GetTestResults()
        {
            return new List<TestResult>();
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        protected override void DoRun(Record record)
        {
            AddmlKey addmlKey = new AddmlKey();

            // vedlegg_dok/vedlegg_dok/melding_nr | 12345
            // vedlegg_dok/vedlegg_dok/vedlegg_nr | 1

            // vedlegg_dok/vedlegg_dok/[melding_nr|vedlegg_nr]=12345|1

            foreach (var field in record.Fields)
            {
                if (field.Definition.IsPartOfPrimaryKey())
                {
                    addmlKey.Add(field.Definition.GetIndex(), field.Value);
                }
            }

            if (!addmlKey.IsEmpty())
            {
                if (_primaryKeys.ContainsKey(addmlKey.Key()))
                {
                    _primaryKeys[addmlKey.Key()].Add(addmlKey.Value());
                }
                else
                {
                    _primaryKeys.Add(addmlKey.Key(), new HashSet<string>() {addmlKey.Value()});
                }
            }
        }

        protected override void DoRun(Field field)
        {
        }
    }

    public class AddmlKey
    {
        public const string FieldDelimiter = "|";

        readonly StringBuilder _keyBuilder = new StringBuilder();
        readonly StringBuilder _valueBuilder = new StringBuilder();

        public void Add(FieldIndex key, string value)
        {
            if (!IsEmpty())
            {
                _keyBuilder.Append(FieldDelimiter);
                _valueBuilder.Append(FieldDelimiter);
            }
            _keyBuilder.Append(key.ToString());
            _valueBuilder.Append(value);
        }

        public string Key() => _keyBuilder.ToString();
        public string Value() => _valueBuilder.ToString();

        public bool IsEmpty()
        {
            return _keyBuilder.Length == 0;
        }
    }

}