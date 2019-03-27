using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class AddmlRecordDefinition
    {
        public AddmlFlatFileDefinition AddmlFlatFileDefinition { get; }

        public string Name { get; }

        public int? RecordLength { get; }

        public string RecordDefinitionFieldValue { get; }
        public List<AddmlForeignKey> ForeignKeys { get; }

        public List<AddmlFieldDefinition> PrimaryKey { get; private set; }

        public List<AddmlFieldDefinition> AddmlFieldDefinitions { get; }

        public List<string> Processes { get; }

        public int? HeaderLevel { get; }

        public AddmlRecordDefinition(AddmlFlatFileDefinition addmlFlatFileDefinition,
            string name,
            int? recordLength,
            string recordDefinitionFieldValue,
            List<AddmlForeignKey> foreignKeys,
            List<string> processes,
            int? headerLevel)
        {
            AddmlFlatFileDefinition = addmlFlatFileDefinition;
            Name = name;
            RecordLength = recordLength;
            RecordDefinitionFieldValue = recordDefinitionFieldValue;
            ForeignKeys = foreignKeys;
            AddmlFieldDefinitions = new List<AddmlFieldDefinition>();
            Processes = processes;
            HeaderLevel = headerLevel;
        }

        public AddmlFieldDefinition AddAddmlFieldDefinition(string name,
            int? startPosition,
            int? fixedLength,
            DataType dataType,
            bool isUnique,
            bool isNullable,
            int? minLength,
            int? maxLength,
            List<string> processes,
            List<AddmlCode> codes,
            bool isPartOfPrimaryKey)
        {
            AddmlFieldDefinition addmlFieldDefinition = new AddmlFieldDefinition(
                name,
                startPosition,
                fixedLength,
                dataType,
                isUnique,
                isNullable,
                minLength,
                maxLength,
                this,
                processes,
                codes);

            if (isPartOfPrimaryKey)
            {
                if (PrimaryKey == null)
                {
                    PrimaryKey = new List<AddmlFieldDefinition>();
                }
                PrimaryKey.Add(addmlFieldDefinition);
            }

            

            AddmlFieldDefinitions.Add(addmlFieldDefinition);

            return addmlFieldDefinition;
        }

        public string Key()
        {
            return AddmlFlatFileDefinition.Name + "_" + Name;
        }

        public RecordIndex GetIndex()
        {
            return new RecordIndex(AddmlFlatFileDefinition.Name, Name);
        }

        public bool HasProcessWithName(string processName)
        {
            return Processes.Contains(processName);
        }

        public void AddProcess(string processName)
        {
            if (!HasProcessWithName(processName))
                Processes.Add(processName);
        }
    }
}