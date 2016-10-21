using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Serilog;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FlatFileReader : IFlatFileReader
    {
        private readonly FixedFormatReader _fixedFormatReader;

        private readonly AddmlRecordDefinition _addmlRecordDefinition;

        public FlatFileReader(FlatFile file)
        {
            // TODO: Add support for multiple AddmlRecordDefinitions?
            if (file.Definition.AddmlRecordDefinitions.Count < 1)
            {
                throw new ArgumentException("file.Definition.AddmlRecordDefinitions must contain exacly one AddmlRecordDefinition. Was " + file.Definition.AddmlRecordDefinitions.Count);
            }
            if (file.Definition.AddmlRecordDefinitions.Count > 1)
            {
                Log.Warning("file.Definition.AddmlRecordDefinitions contains more than one AddmlRecordDefinition. First one will be used.");
            }
            _addmlRecordDefinition = file.Definition.AddmlRecordDefinitions[0];

            FileStream fileStream = file.Definition.FileInfo.OpenRead();
            int recordLength = _addmlRecordDefinition.RecordLength;

            Encoding encoding = file.Definition.Encoding;
            StreamReader streamReader = new StreamReader(fileStream, encoding);

            List<int> fieldLengths = new List<int>();
            foreach (AddmlFieldDefinition addmlFieldDefinition in _addmlRecordDefinition.AddmlFieldDefinitions)
            {
                fieldLengths.Add(addmlFieldDefinition.FixedLength.Value);
            }

            _fixedFormatReader = new FixedFormatReader(streamReader, recordLength, fieldLengths);
        }

        public bool HasMoreRecords()
        {
            return _fixedFormatReader.HasMoreRecords();
        }

        public Record GetNextRecord()
        {
            List<string> fieldValues = _fixedFormatReader.GetNextValue();
            List<Field> fields = CreateFields(fieldValues);

            return new Record(_addmlRecordDefinition, fields);
        }

        private List<Field> CreateFields(List<string> fieldValues)
        {
            List<Field> fields = new List<Field>();
            for (int i = 0; i < fieldValues.Count; i++)
            {
                fields.Add(new Field(_addmlRecordDefinition.AddmlFieldDefinitions[i], fieldValues[i]));
            }
            return fields;
        }
    }
}
