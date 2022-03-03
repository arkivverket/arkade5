using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Internal;
using Serilog;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class AddmlFlatFileDefinition
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get; }
        public AddmlDefinitionFlatFileName FileName { get; }
        public FileInfo FileInfo { get; }
        public Separator RecordSeparator { get; }
        public Separator FieldSeparator { get; }
        public string QuotingChar { get; }

        public Encoding Encoding { get; }
        public string RecordDefinitionFieldIdentifier { get; }
        public int? NumberOfRecords { get; }
        public Checksum Checksum { get; }
        public List<AddmlRecordDefinition> AddmlRecordDefinitions { get; }
        public AddmlFlatFileFormat Format { get; }
        public List<string> Processes { get; }

        private readonly FlatFileIndex _index;

        public AddmlFlatFileDefinition(string name,
            AddmlDefinitionFlatFileName fileName,
            FileInfo fileInfo,
            string recordSeparator,
            string fieldSeparator,
            string quotingChar,
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
            QuotingChar = quotingChar;
            Encoding = Encodings.GetEncoding(charset);
            RecordDefinitionFieldIdentifier = recordDefinitionFieldIdentifier;
            NumberOfRecords = numberOfRecords;
            AddmlRecordDefinitions = new List<AddmlRecordDefinition>();
            Checksum = checksum;
            Format = format;
            Processes = processes;

            _index = new FlatFileIndex(name);
        }

        internal AddmlRecordDefinition AddAddmlRecordDefinition(string name, int? recordLength, string recordDefinitionFieldValue, List<AddmlForeignKey> foreignKeys, int? headerLevel, List<string> processes)
        {
            AddmlRecordDefinition addmlFieldDefinition = new AddmlRecordDefinition(this,
                name,
                recordLength,
                recordDefinitionFieldValue,
                foreignKeys,
                headerLevel,
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
                recordDefsWithControlForeignKey.AddRange(flatFileDef.GetRecordDefinitionsWithProcess(A_16_ControlForeignKey.Name));
            }

            Log.Debug($"Number of {A_16_ControlForeignKey.Name} processes found: {recordDefsWithControlForeignKey.Count}");

            foreach (var recordDef in recordDefsWithControlForeignKey)
            {
                foreach (var foreignKey in recordDef.ForeignKeys)
                {
                    foreach (var fieldDef in foreignKey.ForeignKeyReferenceFields)
                    {
                        fieldDef.AddmlRecordDefinition.AddProcess(AI_01_CollectPrimaryKey.Name);
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


        /// <summary>
        /// Field position is zero-based
        /// </summary>
        /// <returns>null if recordDefinitionFieldIdentifier is not defined</returns>
        public int? GetRecordIdentifierPosition()
        {
            if (string.IsNullOrEmpty(RecordDefinitionFieldIdentifier))
            {
                return null;
            }

            int recordIdentifierPosition = 0;
            foreach (var fieldDef in AddmlRecordDefinitions[0].AddmlFieldDefinitions) // assume recordDefinitionFieldIdentifier is in same position across all record types
            {
                if (fieldDef.Name == RecordDefinitionFieldIdentifier)
                {
                    break;
                }
                recordIdentifierPosition++;
            }
            return recordIdentifierPosition;
                
        }
    }

    public enum AddmlFlatFileFormat
    {
        Fixed,
        Delimiter
    }
}