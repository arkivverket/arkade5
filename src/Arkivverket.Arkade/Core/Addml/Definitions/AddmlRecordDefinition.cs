using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.ExternalModels.Addml;

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
            FieldIndex foreignKeyIndex,
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
                foreignKeyIndex,
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
    }
}