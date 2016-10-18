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
            List<AddmlFieldDefinition> primaryKey,
            List<string> processes)
        {
            AddmlFlatFileDefinition = addmlFlatFileDefinition;
            RecordLength = recordLength;
            PrimaryKey = primaryKey;
            AddmlFieldDefinitions = new List<AddmlFieldDefinition>();
            Processes = processes;
        }

        internal void AddAddmlFieldDefinition(string name, int? startPosition, int? fixedLength, string fieldTypeString,
            bool isPrimaryKey, bool isUnique, bool isNullable, int? minLength, int? maxLength,
            AddmlFieldDefinition foreignKey,
            List<string> processes)
        {
            AddmlFieldDefinition addmlFieldDefinition = new AddmlFieldDefinition(
                name, startPosition, fixedLength, fieldTypeString, isPrimaryKey, isUnique, isNullable, minLength,
                maxLength,
                foreignKey, this, processes);
            AddmlFieldDefinitions.Add(addmlFieldDefinition);
        }
    }
}