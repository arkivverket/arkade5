using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlForeignKey : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 0); // TODO: Assign correct test number

        public const string Name = "Control_ForeignKey";

        private static readonly ILogger Log = Serilog.Log.ForContext<ControlForeignKey>();

        private readonly Dictionary<string, AddmlForeignKey> _foreignKeys = new Dictionary<string, AddmlForeignKey>();

        public Dictionary<string, HashSet<string>> CollectedPrimaryKeys = new Dictionary<string, HashSet<string>>();

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
            return Messages.ControlForeignKeyDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override void DoRun(Record record)
        {
            foreach (AddmlForeignKey foreignKey in record.Definition.ForeignKeys)
            {
                var foreignKeyValues = new List<AddmlForeignKeyValue>();

                foreach (Field field in record.Fields)
                {
                    if (field.IsPartOfForeignKey(foreignKey))
                    {
                        foreignKeyValues.Add(new AddmlForeignKeyValue(field.Definition.GetIndex(), field.Value));
                    }
                }

                foreignKey.AddValue(foreignKeyValues);

                if (!_foreignKeys.ContainsKey(foreignKey.GetForeignKeyReferenceIndexesAsString()))
                {
                    _foreignKeys[foreignKey.GetForeignKeyReferenceIndexesAsString()] = foreignKey;
                }
            }
        }


        protected override void DoRun(Field field)
        {
        }

        protected override void DoRun(FlatFile flatFile)
        {
        }

        protected override void DoEndOfFile()
        {
        }

        protected override List<TestResult> GetTestResults()
        {
            var results = new List<TestResult>();
            foreach (KeyValuePair<string, AddmlForeignKey> item in _foreignKeys)
            {
                string index = item.Key;
                AddmlForeignKey foreignKey = item.Value;
                if (CollectedPrimaryKeys.ContainsKey(index))
                {
                    foreach (string value in foreignKey.Values)
                    {
                        HashSet<string> primaryKeyValues = CollectedPrimaryKeys[index];
                        if (!primaryKeyValues.Contains(value))
                        {

                            string errorMessageTemplate = Messages.ControlForeignKeyMessage1;
                            if (foreignKey.IsCombinedForeignKey())
                            {
                                errorMessageTemplate = Messages.ControlForeignKeyMessage1Combined;
                            }

                            results.Add(new TestResult(ResultType.Error, 
                                AddmlLocation.FromFieldIndex(foreignKey.ForeignKeyIndexes),
                                string.Format(errorMessageTemplate, 
                                    PrettyPrintValue(value), 
                                    AddmlLocation.FromFieldIndex(foreignKey.ForeignKeyReferenceIndexes))));
                        }
                    }
                }
                else
                {
                    results.Add(new TestResult(ResultType.Error, AddmlLocation.FromFieldIndex(foreignKey.ForeignKeyIndexes),
                        string.Format(Messages.ControlForeignKeyMessage2, 
                        AddmlLocation.FromFieldIndex(foreignKey.ForeignKeyReferenceIndexes))));
                }
            }
            return results;
        }

        private string PrettyPrintValue(string input)
        {
            if (input.Contains(AddmlKey.FieldDelimiter))
            {
                return input.Replace(AddmlKey.FieldDelimiter, ", ");
            }
            return input;
        }

        public void GetCollectedPrimaryKeys(CollectPrimaryKey collectPrimaryKeyProcess)
        {
            CollectedPrimaryKeys = collectPrimaryKeyProcess._primaryKeys;
        }
    }
}