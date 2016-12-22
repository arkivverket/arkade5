using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Arkivverket.Arkade.Core.Addml.Processes;
using Serilog;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlFlatFileDefinition
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

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

        public static void InsertForeignKeyProcessInFilesWithReferencedPrimaryKey(List<AddmlFlatFileDefinition> addmlFlatFileDefinitions)
        {
            Log.Debug("Inserting foreign key process in definitions with a referenced primary key");
            var fieldDefsWithControlForeignKey = new List<AddmlFieldDefinition>();
            foreach (var flatFileDef in addmlFlatFileDefinitions)
            {
                fieldDefsWithControlForeignKey.AddRange(flatFileDef.GetFieldDefinitionsWithProcess(ControlForeignKey.Name));
            }

            Log.Debug($"Number of {ControlForeignKey.Name} processes found: {fieldDefsWithControlForeignKey.Count}");

            foreach (var fieldDef in fieldDefsWithControlForeignKey)
            {
                fieldDef.ForeignKey.AddProcess(ControlForeignKey.Name);
            }

        }

        private IEnumerable<AddmlFieldDefinition> GetFieldDefinitionsWithProcess(string processName)
        {
            var definitionsWithProcess = new List<AddmlFieldDefinition>();
            foreach (var recordDef in AddmlRecordDefinitions)
            {
                definitionsWithProcess.AddRange(recordDef.GetFieldDefinitionsWithProcess(processName));
            }
            return definitionsWithProcess;
        }
    }

    public enum AddmlFlatFileFormat
    {
        Fixed,
        Delimiter
    }
}