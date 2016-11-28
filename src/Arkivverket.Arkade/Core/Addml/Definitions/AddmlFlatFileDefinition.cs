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
        public Checksum Checksum { get; }
        public List<AddmlRecordDefinition> AddmlRecordDefinitions { get; }
        public AddmlFlatFileFormat Format { get; }
        public List<string> Processes { get; }

        private readonly FlatFileIndex _index;

        public AddmlFlatFileDefinition(string name,
            string fileName,
            FileInfo fileInfo,
            string recordSeparator,
            string fieldSeparator,
            string charset,
            string recordDefinitionFieldIdentifier,
            int? numberOfRecords,
            Checksum checksum,
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
            Checksum = checksum;
            Format = format;
            Processes = processes;

            _index = new FlatFileIndex(name);
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

        public FlatFileIndex GetIndex()
        {
            return _index;
        }
    }

    public enum AddmlFlatFileFormat
    {
        Fixed,
        Delimiter
    }
}