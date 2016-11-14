using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Tests;
using Serilog;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlForeignKey : IAddmlProcess
    {
        public const string Name = "Control_ForeignKey";
        public const string Description = "Description of " + Name;

        private ILogger _log = Log.ForContext<ControlForeignKey>();

        private readonly List<ForeignKeyValue> _foreignKeys = new List<ForeignKeyValue>();

        private readonly Dictionary<string, PrimaryKeyValue> _primaryKeys = new Dictionary<string, PrimaryKeyValue>();

        public string GetName()
        {
            return Name;
        }

        public string GetDescription()
        {
            return Description;
        }

        public void Run(Field field)
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

        public void Run(FlatFile flatFile)
        {
        }

        public void Run(Record record)
        {
        }

        public TestRun GetTestRun()
        {
            var testRun = new TestRun(GetName(),TestType.Content);
            testRun.Results = CreateTestResults();
            return testRun;
        }

        public void EndOfFile()
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

        private List<TestResult> CreateTestResults()
        {
            var results = new List<TestResult>();
            foreach (ForeignKeyValue foreignKeyValue in _foreignKeys)
            {
                if (_primaryKeys.ContainsKey(foreignKeyValue.ReferencingField))
                {
                    PrimaryKeyValue primaryKeyValue = _primaryKeys[foreignKeyValue.ReferencingField];
                    if (!primaryKeyValue.HasValue(foreignKeyValue.Value))
                    {
                        results.Add(new TestResult(ResultType.Error, new Location(""), "Invalid foreign key: " + foreignKeyValue.Value));
                    }
                }
                else
                {
                    results.Add(new TestResult(ResultType.Error, new Location(""), "Cannot find referenced field [" + foreignKeyValue.ReferencingField + "] from foreign key [" + foreignKeyValue.Field + "]."));
                }
            }
            return results;
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