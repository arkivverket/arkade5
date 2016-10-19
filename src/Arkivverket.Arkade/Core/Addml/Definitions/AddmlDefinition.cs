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
        }

        public List<FlatFile> GetFlatFiles()
        {
            return AddmlFlatFileDefinitions.Select(
                addmlFlatFileDefinition => new FlatFile(addmlFlatFileDefinition)).ToList();
        }

        public Dictionary<string, List<string>> GetFileProcessesGroupedByFile()
        {
            return AddmlFlatFileDefinitions.ToDictionary(d => d.Name, d => d.Processes);
        }

        public Dictionary<string, List<string>> GetRecordProcessesGroupedByRecord()
        {
            Dictionary<string, List<string>> fieldProcessNamesGroupedByRecord = new Dictionary<string, List<string>>();
            foreach (var flatFileDefinition in AddmlFlatFileDefinitions)
            {
                foreach (var recordDefinition in flatFileDefinition.AddmlRecordDefinitions)
                {
                    fieldProcessNamesGroupedByRecord.Add(recordDefinition.Key(), recordDefinition.Processes);
                }
            }
            return fieldProcessNamesGroupedByRecord;
        }

        public Dictionary<string, List<string>> GetFieldProcessesGroupedByField()
        {
            Dictionary<string, List<string>> fieldProcessNamesGroupedByField = new Dictionary<string, List<string>>();
            foreach (var flatFileDefinition in AddmlFlatFileDefinitions)
            {
                foreach (var recordDefinition in flatFileDefinition.AddmlRecordDefinitions)
                {
                    foreach (var fieldDefinition in recordDefinition.AddmlFieldDefinitions)
                    {
                        fieldProcessNamesGroupedByField.Add(fieldDefinition.Key(), fieldDefinition.Processes);
                    }
                }
            }
            return fieldProcessNamesGroupedByField;
        }
    }
}