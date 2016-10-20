using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlRecordDefinition
    {
        public AddmlFlatFileDefinition AddmlFlatFileDefinition { get; }

        public string Name { get; }

        public int RecordLength { get; }

        public List<AddmlFieldDefinition> PrimaryKey { get; }

        public List<AddmlFieldDefinition> AddmlFieldDefinitions { get; }

        public List<string> Processes { get; }

        public AddmlRecordDefinition(AddmlFlatFileDefinition addmlFlatFileDefinition,
            string name,
            int recordLength,
            List<string> processes)
        {
            AddmlFlatFileDefinition = addmlFlatFileDefinition;
            Name = name;
            RecordLength = recordLength;
            PrimaryKey = new List<AddmlFieldDefinition>();
            AddmlFieldDefinitions = new List<AddmlFieldDefinition>();
            Processes = processes;
        }

        internal AddmlFieldDefinition AddAddmlFieldDefinition(string name,
            int? startPosition,
            int? fixedLength,
            FieldType fieldType,
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
                fieldType,
                isUnique,
                isNullable,
                minLength,
                maxLength,
                foreignKey,
                this,
                processes);

            if (isPartOfPrimaryKey)
            {
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