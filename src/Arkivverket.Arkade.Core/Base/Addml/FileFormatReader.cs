using System;
using System.Collections;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public abstract class FileFormatReader : IRecordEnumerator
    {
        private readonly AddmlRecordDefinition _addmlRecordDefinition;

        private readonly Dictionary<string, AddmlRecordDefinition> _addmlRecordDefinitions =
            new Dictionary<string, AddmlRecordDefinition>();

        protected FileFormatReader(AddmlFlatFileDefinition addmlFlatFileDefinition)
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

        public abstract void Dispose();
        public abstract bool MoveNext();
        public abstract void Reset();
        public abstract Record Current { get; }

        object IEnumerator.Current => Current;
    }
}