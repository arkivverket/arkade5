using System.Collections.Generic;
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

        private readonly Dictionary<string, PrimaryKeyValue> _primaryKeys = new Dictionary<string, PrimaryKeyValue>();

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
            // TODO: IsPartOfPrimaryKey is probably not correct here!
            if (field.Definition.IsPartOfPrimaryKey())
            {
                _log.Debug("Adding primary key {fieldKey} with value {fieldValue}", field.Definition.Key(), field.Value);
                AddPrimaryKey(field);
            }
            if (field.Definition.ForeignKey != null)
            {
                _log.Debug("Adding foreign key {fieldKey} with value {fieldValue}", field.Definition.Key(), field.Value);
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
                        results.Add(new TestResult(ResultType.Error, new Location(""),
                            string.Format(Messages.ControlForeignKeyMessage1, foreignKeyValue.Value)));
                    }
                }
                else
                {
                    results.Add(new TestResult(ResultType.Error, new Location(""),
                        string.Format(Messages.ControlForeignKeyMessage1, foreignKeyValue.ReferencingField,
                            foreignKeyValue.Field)));
                }
            }
            return results;
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        private void AddPrimaryKey(Field field)
        {
            PrimaryKeyValue primaryKeyValue;
            string key = field.Definition.Key();
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
            private string _field;

            public PrimaryKeyValue(Field field)
            {
                _field = field.Definition.Key();
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
            public string Field { get; set; }
            public string Value { get; set; }
            public string ReferencingField { get; set; }

            public ForeignKeyValue(Field field)
            {
                Field = field.Definition.Key();
                Value = field.Value;
                ReferencingField = field.Definition.ForeignKey.Key();
            }
        }
    }
}