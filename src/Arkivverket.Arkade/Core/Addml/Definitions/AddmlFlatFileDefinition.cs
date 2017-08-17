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

        internal AddmlRecordDefinition AddAddmlRecordDefinition(string name, int? recordLength, string recordDefinitionFieldValue, List<AddmlForeignKey> foreignKeys, List<string> processes)
        {
            AddmlRecordDefinition addmlFieldDefinition = new AddmlRecordDefinition(this,
                name,
                recordLength,
                recordDefinitionFieldValue,
                foreignKeys,
                processes);
            AddmlRecordDefinitions.Add(addmlFieldDefinition);
            return addmlFieldDefinition;
        }

        public FlatFileIndex GetIndex()
        {
            return _index;
        }

        public static void InsertCollectPrimaryKeyProcessInDefinitionsReferencedFromAForeignKeyWithControlProcess(List<AddmlFlatFileDefinition> addmlFlatFileDefinitions)
        {
            Log.Debug("Inserting CollectPrimaryKey process in definitions with a referenced primary key from records with ControlForeignKey process.");
            var recordDefsWithControlForeignKey = new List<AddmlRecordDefinition>();
            foreach (var flatFileDef in addmlFlatFileDefinitions)
            {
                recordDefsWithControlForeignKey.AddRange(flatFileDef.GetRecordDefinitionsWithProcess(ControlForeignKey.Name));
            }

            Log.Debug($"Number of {ControlForeignKey.Name} processes found: {recordDefsWithControlForeignKey.Count}");

            foreach (var recordDef in recordDefsWithControlForeignKey)
            {
                foreach (var foreignKey in recordDef.ForeignKeys)
                {
                    foreach (var fieldDef in foreignKey.ForeignKeyReferenceFields)
                    {
                        fieldDef.AddmlRecordDefinition.AddProcess(CollectPrimaryKey.Name);
                    }
                }
            }
        }

        private IEnumerable<AddmlRecordDefinition> GetRecordDefinitionsWithProcess(string processName)
        {
            var definitionsWithProcess = new List<AddmlRecordDefinition>();
            foreach (var recordDef in AddmlRecordDefinitions)
            {
                if (recordDef.HasProcessWithName(processName))
                    definitionsWithProcess.Add(recordDef);
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