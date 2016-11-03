using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlRecordDefinition
    {
        public AddmlFlatFileDefinition AddmlFlatFileDefinition { get; }

        public string Name { get; }

        public int? RecordLength { get; }

        public string RecordDefinitionFieldValue { get; }

        public List<AddmlFieldDefinition> PrimaryKey { get; private set; }

        public List<AddmlFieldDefinition> AddmlFieldDefinitions { get; }

        public List<string> Processes { get; }

        public AddmlRecordDefinition(AddmlFlatFileDefinition addmlFlatFileDefinition,
            string name,
            int? recordLength,
            string recordDefinitionFieldValue,
            List<string> processes)
        {
            AddmlFlatFileDefinition = addmlFlatFileDefinition;
            Name = name;
            RecordLength = recordLength;
            RecordDefinitionFieldValue = recordDefinitionFieldValue;
            AddmlFieldDefinitions = new List<AddmlFieldDefinition>();
            Processes = processes;
        }

        public AddmlFieldDefinition AddAddmlFieldDefinition(string name,
            int? startPosition,
            int? fixedLength,
            DataType dataType,
            bool isUnique,
            bool isNullable,
            int? minLength,
            int? maxLength,
            AddmlFieldDefinition foreignKey,
            List<string> processes,
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
                foreignKey,
                this,
                processes);

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
    }
}