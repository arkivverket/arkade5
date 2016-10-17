using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlFlatFileDefinition
    {
        public string Name { get; }
        public string FileName { get; }
        public int RecordLength { get; }
        public string RecordSeparator { get; }
        public string FieldSeparator { get; }
        public string Charset { get; }

        public int NumberOfOccurences { get; }
        public string ChecksumAlgorithm { get; }
        public string ChecksumValue { get; }

        public List<AddmlFieldDefinition> AddmlFieldDefinitions { get; }

        // FileProcesses
        // RecordProcesses

        public AddmlFlatFileDefinition(string name,
            string fileName,
            int recordLength,
            string recordSeparator,
            string charset)
        {
            Name = name;
            FileName = fileName;
            RecordLength = recordLength;
            RecordSeparator = recordSeparator;
            Charset = charset;
            AddmlFieldDefinitions = new List<AddmlFieldDefinition>();
        }

        internal void AddAddmlFieldDefinition(string name, int? startPosition, int? fixedLength, string fieldTypeString,
            bool isPrimaryKey, bool isUnique, bool isNullable, int? minLength, int? maxLength, AddmlFieldDefinition foreignKey)
        {
            AddmlFieldDefinition addmlFieldDefinition = new AddmlFieldDefinition(
                    name, startPosition, fixedLength, fieldTypeString, isPrimaryKey, isUnique, isNullable, minLength, maxLength,
                    foreignKey, this);
            AddmlFieldDefinitions.Add(addmlFieldDefinition);
        }
    }
}
