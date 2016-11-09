using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlFlatFileDefinition
    {
        public string Name { get; }
        public string FileName { get; }
        public FileInfo FileInfo { get; }
        public RecordSeparator RecordSeparator { get; }
        public Encoding Encoding { get; }
        public string RecordDefinitionFieldIdentifier { get; }
        public int? NumberOfRecords { get; }
        public string ChecksumAlgorithm { get; }
        public string ChecksumValue { get; }

        public List<AddmlRecordDefinition> AddmlRecordDefinitions { get; }

        public List<string> Processes { get; }

        public AddmlFlatFileDefinition(string name,
            string fileName,
            FileInfo fileInfo,
            string recordSeparator,
            string charset,
            string recordDefinitionFieldIdentifier,
            int? numberOfRecords,
            List<string> processes)
        {
            Name = name;
            FileName = fileName;
            FileInfo = fileInfo;
            RecordSeparator = string.IsNullOrEmpty(recordSeparator) ? null : new RecordSeparator(recordSeparator);
            Encoding = Encodings.GetEncoding(charset);
            RecordDefinitionFieldIdentifier = recordDefinitionFieldIdentifier;
            NumberOfRecords = numberOfRecords;
            AddmlRecordDefinitions = new List<AddmlRecordDefinition>();
            Processes = processes;
        }

        internal AddmlRecordDefinition AddAddmlRecordDefinition(string name, int? recordLength,
            string recordDefinitionFieldValue, List<string> processes)
        {
            AddmlRecordDefinition addmlFieldDefinition = new AddmlRecordDefinition(this,
                name,
                recordLength,
                recordDefinitionFieldValue,
                processes);
            AddmlRecordDefinitions.Add(addmlFieldDefinition);
            return addmlFieldDefinition;
        }

        public string Key()
        {
            return Name;
        }
    }
}