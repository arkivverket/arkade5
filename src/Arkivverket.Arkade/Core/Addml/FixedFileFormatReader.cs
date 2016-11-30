using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FixedFileFormatReader : IRecordEnumerator
    {
        // TODO jostein: FixedFileFormatReader and FixedFormatReader should be merged together
        // TODO jostein: Functionality in FileFormatReader should be used
        private readonly FixedFormatReader _fixedFormatReader;

        private readonly Dictionary<string, AddmlRecordDefinition> _addmlRecordDefinitions;
        private Record _currentRecord;

        public FixedFileFormatReader(FlatFile file) : this(file, file.Definition.FileInfo.OpenRead())
        {
        }

        public FixedFileFormatReader(FlatFile file, Stream stream)
        {
            _addmlRecordDefinitions = new Dictionary<string, AddmlRecordDefinition>();

            AddmlFlatFileDefinition flatFileDefinition = file.Definition;
            List<AddmlRecordDefinition> addmlRecordDefinitions = flatFileDefinition.AddmlRecordDefinitions;

            int? recordLength = null;
            foreach (AddmlRecordDefinition addmlRecordDefinition in addmlRecordDefinitions)
            {
                if (!recordLength.HasValue)
                {
                    recordLength = addmlRecordDefinition.RecordLength;
                }

                if (recordLength != addmlRecordDefinition.RecordLength)
                {
                    throw new ArkadeException("Different record lengths in same file is not supported");
                }

                string recordDefinitionFieldValue = addmlRecordDefinition.RecordDefinitionFieldValue;
                if (addmlRecordDefinitions.Count == 1 && recordDefinitionFieldValue == null)
                {
                    recordDefinitionFieldValue = "";
                }

                _addmlRecordDefinitions.Add(recordDefinitionFieldValue, addmlRecordDefinition);
            }

            if (!recordLength.HasValue)
            {
                throw new ArkadeException("FlatFileReader requires recordLength");
            }

            Encoding encoding = file.Definition.Encoding;
            StreamReader streamReader = new StreamReader(stream, encoding);

            var fixedFormatDefinition = new FixedFormatReader.FixedFormatConfig();
            Tuple<int?, int?> identifierStartPositionAndLength = GetIdentifierStartPositionAndLength(flatFileDefinition);
            fixedFormatDefinition.IdentifierStartPosition = identifierStartPositionAndLength.Item1;
            fixedFormatDefinition.IdentifierLength = identifierStartPositionAndLength.Item2;
            fixedFormatDefinition.RecordLength = recordLength.Value;
            fixedFormatDefinition.RecordSparator = flatFileDefinition.RecordSeparator;
            fixedFormatDefinition.RecordDefinitions = GetRecordFefinitions(flatFileDefinition);

            _fixedFormatReader = new FixedFormatReader(streamReader, fixedFormatDefinition);
        }

        private List<FixedFormatReader.FixedFormatRecordConfig> GetRecordFefinitions(AddmlFlatFileDefinition flatFileDefinition)
        {
            List<FixedFormatReader.FixedFormatRecordConfig> recordDefinitions = new List<FixedFormatReader.FixedFormatRecordConfig>();
            foreach (AddmlRecordDefinition addmlRecordDefinition in flatFileDefinition.AddmlRecordDefinitions)
            {
                FixedFormatReader.FixedFormatRecordConfig rd
                    = new FixedFormatReader.FixedFormatRecordConfig();
                rd.RecordIdentifier = addmlRecordDefinition.RecordDefinitionFieldValue;
                rd.FieldLengths = GetFieldLengths(addmlRecordDefinition);
                recordDefinitions.Add(rd);
            }

            return recordDefinitions;
        }

        private List<int> GetFieldLengths(AddmlRecordDefinition addmlRecordDefinition)
        {
            List<int> fieldLengths = new List<int>();
            foreach (AddmlFieldDefinition fd in addmlRecordDefinition.AddmlFieldDefinitions)
            {
                int? fixedLength = fd.FixedLength;
                if (!fixedLength.HasValue)
                {
                    throw new ArkadeException("FixedLength must be present in ADDML to use FlatFileReader");
                }
                fieldLengths.Add(fixedLength.Value);
            }

            return fieldLengths;
        }

        private Tuple<int?, int?> GetIdentifierStartPositionAndLength(AddmlFlatFileDefinition flatFileDefinition)
        {
            string recordDefinitionFieldIdentifier = flatFileDefinition.RecordDefinitionFieldIdentifier;

            if (recordDefinitionFieldIdentifier == null)
            {
                return new Tuple<int?, int?>(null, null);
            }

            foreach (AddmlRecordDefinition addmlRecordDefinition in flatFileDefinition.AddmlRecordDefinitions)
            {
                foreach (AddmlFieldDefinition addmlFieldDefinition in addmlRecordDefinition.AddmlFieldDefinitions)
                {
                    if (addmlFieldDefinition.Name == recordDefinitionFieldIdentifier)
                    {
                        // ADDML startPosition is 1 based
                        int? startPosition = addmlFieldDefinition.StartPosition - 1;
                        int? length = addmlFieldDefinition.FixedLength;
                        return new Tuple<int?, int?>(startPosition, length);
                    }
                }
            }

            return new Tuple<int?, int?>(null, null);
        }

        private bool HasMoreRecords()
        {
            return _fixedFormatReader.HasMoreRecords();
        }

        private Record GetNextRecord()
        {
            Tuple<string, List<string>> fieldIdentifierAndValues = _fixedFormatReader.GetNextValue();

            List<Field> fields = CreateFields(fieldIdentifierAndValues);
            AddmlRecordDefinition addmlRecordDefinition = _addmlRecordDefinitions[fieldIdentifierAndValues.Item1];
            return new Record(addmlRecordDefinition, fields);
        }

        private List<Field> CreateFields(Tuple<string, List<string>> fieldIdentifierAndValues)
        {
            List<Field> fields = new List<Field>();

            for (int i = 0; i < fieldIdentifierAndValues.Item2.Count; i++)
            {
                List<string> fieldValues = fieldIdentifierAndValues.Item2;
                AddmlRecordDefinition addmlRecordDefinition = _addmlRecordDefinitions[fieldIdentifierAndValues.Item1];

                fields.Add(new Field(addmlRecordDefinition.AddmlFieldDefinitions[i], fieldValues[i]));
            }
            return fields;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            bool hasMoreRecords = HasMoreRecords();
            if (hasMoreRecords)
            {
                _currentRecord = GetNextRecord();
            }
            return hasMoreRecords;
        }

        public void Reset()
        {
        }

        public Record Current
        {
            get { return _currentRecord; } 
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
