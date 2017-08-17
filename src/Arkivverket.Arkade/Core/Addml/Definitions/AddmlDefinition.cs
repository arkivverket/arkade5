using System.Collections.Generic;
using System.Linq;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlDefinition
    {

        public List<AddmlFlatFileDefinition> AddmlFlatFileDefinitions { get; }

        public AddmlDefinition(List<AddmlFlatFileDefinition> addmlFlatFileDefinitions)
        {
            AddmlFlatFileDefinitions = addmlFlatFileDefinitions;
            InsertCollectPrimaryKeyProcessInDefinitionsReferencedFromAForeignKeyWithControlProcess();
        }

        /// <summary>
        /// The ControlForeignKey ADDML process requires a list of all existing primary keys in order to function correct. 
        /// But the process it self is only defined on the foreign key record. This method makes sure that the CollectPrimaryKey process,
        /// is inserted into all primary key field definitions that are referenced from a foreign key field definition,
        /// where the ControlForeignKey process is defined.
        /// </summary>
        private void InsertCollectPrimaryKeyProcessInDefinitionsReferencedFromAForeignKeyWithControlProcess()
        {
            AddmlFlatFileDefinition.InsertCollectPrimaryKeyProcessInDefinitionsReferencedFromAForeignKeyWithControlProcess(AddmlFlatFileDefinitions);
        }

        public List<FlatFile> GetFlatFiles()
        {
            return AddmlFlatFileDefinitions.Select(
                addmlFlatFileDefinition => new FlatFile(addmlFlatFileDefinition)).ToList();
        }

        public Dictionary<IAddmlIndex, List<string>> GetFileProcessesGroupedByFile()
        {
            return AddmlFlatFileDefinitions.ToDictionary(d => (IAddmlIndex)d.GetIndex(), d => d.Processes);
        }

        public Dictionary<IAddmlIndex, List<string>> GetRecordProcessesGroupedByRecord()
        {
            Dictionary<IAddmlIndex, List<string>> fieldProcessNamesGroupedByRecord = new Dictionary<IAddmlIndex, List<string>>();
            foreach (var flatFileDefinition in AddmlFlatFileDefinitions)
            {
                foreach (var recordDefinition in flatFileDefinition.AddmlRecordDefinitions)
                {
                    fieldProcessNamesGroupedByRecord.Add(recordDefinition.GetIndex(), recordDefinition.Processes);
                }
            }
            return fieldProcessNamesGroupedByRecord;
        }

        public Dictionary<IAddmlIndex, List<string>> GetFieldProcessesGroupedByField()
        {
            Dictionary<IAddmlIndex, List<string>> fieldProcessNamesGroupedByField = new Dictionary<IAddmlIndex, List<string>>();
            foreach (var flatFileDefinition in AddmlFlatFileDefinitions)
            {
                foreach (var recordDefinition in flatFileDefinition.AddmlRecordDefinitions)
                {
                    foreach (var fieldDefinition in recordDefinition.AddmlFieldDefinitions)
                    {
                        fieldProcessNamesGroupedByField.Add(fieldDefinition.GetIndex(), fieldDefinition.Processes);
                    }
                }
            }
            return fieldProcessNamesGroupedByField;
        }
    }
}