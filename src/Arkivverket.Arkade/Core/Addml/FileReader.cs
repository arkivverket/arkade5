using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public abstract class FileReader : IFlatFileReader
    {
        private readonly AddmlRecordDefinition _addmlRecordDefinition;

        private readonly Dictionary<string, AddmlRecordDefinition> _addmlRecordDefinitions =
            new Dictionary<string, AddmlRecordDefinition>();

        protected FileReader(AddmlFlatFileDefinition addmlFlatFileDefinition)
        {
            List<AddmlRecordDefinition> addmlRecordDefinitions = addmlFlatFileDefinition.AddmlRecordDefinitions;

            if (addmlRecordDefinitions.Count < 1)
            {
                throw new ArgumentException("At least 1 addmlFlatFileDefinition must be present. Found: " +
                                            addmlRecordDefinitions.Count);
            }

            if (addmlRecordDefinitions.Count == 1)
            {
                _addmlRecordDefinition = addmlRecordDefinitions[0];
                return;
            }

            foreach (AddmlRecordDefinition addmlRecordDefinition in addmlRecordDefinitions)
            {
                string recordDefinitionFieldValue = addmlRecordDefinition.RecordDefinitionFieldValue;

                if (string.IsNullOrEmpty(recordDefinitionFieldValue))
                {
                    throw new ArgumentException("recordDefinitionFieldValue must be present");
                }

                _addmlRecordDefinitions.Add(recordDefinitionFieldValue, addmlRecordDefinition);
            }
        }

        public abstract bool HasMoreRecords();
        public abstract Record GetNextRecord();

        protected AddmlRecordDefinition GetAddmlRecordDefinition(string recordDefinitionFieldValue)
        {
            if (recordDefinitionFieldValue == null)
            {
                return _addmlRecordDefinition;
            }
            else
            {
                if (!_addmlRecordDefinitions.ContainsKey(recordDefinitionFieldValue))
                {
                    throw new ArgumentException("No AddmlRecordDefinition mapped to " + recordDefinitionFieldValue);
                }

                return _addmlRecordDefinitions[recordDefinitionFieldValue];
            }
        }
    }
}