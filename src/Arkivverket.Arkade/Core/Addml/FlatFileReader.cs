using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FlatFileReader : IFlatFileReader
    {
        private StreamReader _streamReader;
        private int _streamReaderOffset = 0;

        private AddmlRecordDefinition _addmlRecordDefinition;
        private int _nextRecord = 0;

        public FlatFileReader(FlatFile file)
        {
            // TODO: Add support for multiple AddmlRecordDefinitions
            if (file.Definition.AddmlRecordDefinitions.Count > 0)
            {
                _addmlRecordDefinition = file.Definition.AddmlRecordDefinitions[0];
            }

            FileStream fileStream = file.Definition.FileInfo.OpenRead();

            // TODO: Convert charset to Encoding
            Encoding encoding = Encoding.UTF8; //file.Definition.Charset;
            _streamReader = new StreamReader(fileStream, encoding);
        }

        public bool HasMoreRecords()
        {
            return _streamReader.Peek() >= 0;
        }

        public Record GetNextRecord()
        {
            // Read bytes according to recordLength
            int len = _addmlRecordDefinition.RecordLength;
            char[] buffer = new char[len];
            int read = _streamReader.ReadBlock(buffer, 0, len);
            _streamReaderOffset += read;

            string recordString = new string(buffer);

            List<Field> fields = GetFields(recordString);
            return new Record(_addmlRecordDefinition, fields);
        }

        private List<Field> GetFields(string recordString)
        {
            List<Field> fields = new List<Field>();
            int offset = 0;
            foreach (AddmlFieldDefinition addmlFieldDefinition in _addmlRecordDefinition.AddmlFieldDefinitions)
            {
                int len = addmlFieldDefinition.FixedLength.Value;
                string fieldString = recordString.Substring(offset, len);
                offset += len;
                fields.Add(new Field(addmlFieldDefinition, fieldString));
            }
            return fields;
        }
    }
}
