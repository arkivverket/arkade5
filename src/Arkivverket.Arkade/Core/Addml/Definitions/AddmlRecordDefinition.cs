using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlRecordDefinition
    {
        public AddmlFlatFileDefinition AddmlFlatFileDefinition { get; }

        public int RecordLength { get; }

        public List<AddmlFieldDefinition> PrimaryKey { get; }

        public List<AddmlFieldDefinition> AddmlFieldDefinitions { get; }

        public List<string> Processes { get; }

        public AddmlRecordDefinition(AddmlFlatFileDefinition addmlFlatFileDefinition,
            int recordLength,
            List<string> processes)
        {
            AddmlFlatFileDefinition = addmlFlatFileDefinition;
            RecordLength = recordLength;
            PrimaryKey = new List<AddmlFieldDefinition>();
            AddmlFieldDefinitions = new List<AddmlFieldDefinition>();
            Processes = processes;
        }

        internal void AddAddmlFieldDefinition(string name,
            int? startPosition,
            int? fixedLength,
            string fieldTypeString,
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
                fieldTypeString,
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
        }
    }
}