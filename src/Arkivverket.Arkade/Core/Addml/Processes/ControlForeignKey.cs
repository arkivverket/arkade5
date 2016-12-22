using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using Serilog;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlForeignKey : AddmlProcess
    {
        public const string Name = "Control_ForeignKey";

        private readonly List<ForeignKeyValue> _foreignKeys = new List<ForeignKeyValue>();

        private readonly ILogger _log = Log.ForContext<ControlForeignKey>();

        private readonly Dictionary<FieldIndex, PrimaryKeyValue> _primaryKeys = new Dictionary<FieldIndex, PrimaryKeyValue>();

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlForeignKeyDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoRun(Field field)
        {
            if (field.Definition.IsPartOfPrimaryKey())
            {
                AddPrimaryKey(field);
            }
            if (field.Definition.ForeignKey != null)
            {
                _foreignKeys.Add(new ForeignKeyValue(field));
            }
        }

        protected override void DoEndOfFile()
        {
        }

        protected override List<TestResult> GetTestResults()
        {
            var results = new List<TestResult>();
            foreach (ForeignKeyValue foreignKeyValue in _foreignKeys)
            {
                if (_primaryKeys.ContainsKey(foreignKeyValue.ReferencingField))
                {
                    PrimaryKeyValue primaryKeyValue = _primaryKeys[foreignKeyValue.ReferencingField];
                    if (!primaryKeyValue.HasValue(foreignKeyValue.Value))
                    {
                        results.Add(CreateInvalidForeignKeyError(foreignKeyValue));
                    }
                }
                else
                {
                    results.Add(CreateInvalidForeignKeyError(foreignKeyValue));
                }
            }
            return results;
        }

        private static TestResult CreateInvalidForeignKeyError(ForeignKeyValue foreignKeyValue)
        {
            return new TestResult(ResultType.Error, AddmlLocation.FromFieldIndex(foreignKeyValue.Field),
                string.Format(Messages.ControlForeignKeyMessage1, foreignKeyValue.Value,
                    foreignKeyValue.ReferencingField));
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        private void AddPrimaryKey(Field field)
        {
            PrimaryKeyValue primaryKeyValue;
            FieldIndex key = field.Definition.GetIndex();
            if (_primaryKeys.ContainsKey(key))
            {
                primaryKeyValue = _primaryKeys[key];
            }
            else
            {
                primaryKeyValue = new PrimaryKeyValue(field);
                _primaryKeys.Add(key, primaryKeyValue);
            }
            primaryKeyValue.AddValue(field.Value);
        }

        private class PrimaryKeyValue
        {
            private readonly HashSet<string> _values = new HashSet<string>();
            private FieldIndex _field;

            public PrimaryKeyValue(Field field)
            {
                _field = field.Definition.GetIndex();
            }

            public void AddValue(string value)
            {
                _values.Add(value);
            }

            public bool HasValue(string value)
            {
                return _values.Contains(value);
            }
        }

        private class ForeignKeyValue
        {
            public FieldIndex Field { get; set; }
            public string Value { get; set; }
            public FieldIndex ReferencingField { get; set; }

            public ForeignKeyValue(Field field)
            {
                Field = field.Definition.GetIndex();
                Value = field.Value;
                ReferencingField = field.Definition.ForeignKey.GetIndex();
            }
        }
    }
}