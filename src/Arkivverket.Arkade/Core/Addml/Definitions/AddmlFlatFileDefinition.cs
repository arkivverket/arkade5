using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlFlatFileDefinition
    {
        public string Name { get; }
        public string FileName { get; }
        public string RecordSeparator { get; }
        public string Charset { get; }

        public int NumberOfOccurences { get; }
        public string ChecksumAlgorithm { get; }
        public string ChecksumValue { get; }

        public List<AddmlRecordDefinition> AddmlRecordDefinitions { get; }

        public List<string> Processes { get; }

        public AddmlFlatFileDefinition(string name,
            string fileName,
            string recordSeparator,
            string charset,
            List<string> processes)
        {
            Name = name;
            FileName = fileName;
            RecordSeparator = recordSeparator;
            Charset = charset;
            AddmlRecordDefinitions = new List<AddmlRecordDefinition>();
            Processes = processes;
        }

        internal AddmlRecordDefinition AddAddmlRecordDefinition(int recordLength,
            List<string> processes)
        {
            AddmlRecordDefinition addmlFieldDefinition = new AddmlRecordDefinition(this,
                recordLength,
                processes);
            AddmlRecordDefinitions.Add(addmlFieldDefinition);
            return addmlFieldDefinition;
        }
    }
}
