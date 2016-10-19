using System;
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
        public int NumberOfOccurences { get; }
        public string ChecksumAlgorithm { get; }
        public string ChecksumValue { get; }

        public List<AddmlRecordDefinition> AddmlRecordDefinitions { get; }

        public List<string> Processes { get; }

        public AddmlFlatFileDefinition(string name,
            string fileName,
            FileInfo fileInfo,
            string recordSeparator,
            string charset,
            List<string> processes)
        {
            Name = name;
            FileName = fileName;
            FileInfo = fileInfo;
            RecordSeparator = string.IsNullOrEmpty(recordSeparator) ? null : new RecordSeparator(recordSeparator);
            Encoding = Encodings.GetEncoding(charset);
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
