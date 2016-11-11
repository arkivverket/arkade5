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
        public Separator RecordSeparator { get; }
        public Separator FieldSeparator { get; }

        public Encoding Encoding { get; }
        public string RecordDefinitionFieldIdentifier { get; }
        public int? NumberOfRecords { get; }
        public string ChecksumAlgorithm { get; }
        public string ChecksumValue { get; }
        public List<AddmlRecordDefinition> AddmlRecordDefinitions { get; }
        public AddmlFlatFileFormat Format { get; }
        public List<string> Processes { get; }

        public AddmlFlatFileDefinition(string name,
            string fileName,
            FileInfo fileInfo,
            string recordSeparator,
            string fieldSeparator,
            string charset,
            string recordDefinitionFieldIdentifier,
            int? numberOfRecords,
            AddmlFlatFileFormat format,
            List<string> processes)
        {
            Name = name;
            FileName = fileName;
            FileInfo = fileInfo;
            RecordSeparator = string.IsNullOrEmpty(recordSeparator) ? null : new Separator(recordSeparator);
            FieldSeparator = string.IsNullOrEmpty(fieldSeparator) ? null : new Separator(fieldSeparator);
            Encoding = Encodings.GetEncoding(charset);
            RecordDefinitionFieldIdentifier = recordDefinitionFieldIdentifier;
            NumberOfRecords = numberOfRecords;
            AddmlRecordDefinitions = new List<AddmlRecordDefinition>();
            Format = format;
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

    public enum AddmlFlatFileFormat
    {
        Fixed,
        Delimiter
    }
}