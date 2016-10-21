using System.Collections.Generic;
using System.IO;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FixedFormatReader
    {
        private readonly StreamReader _streamReader;
        private readonly int _recordLength;
        private readonly List<int> _fieldLengths;

        public FixedFormatReader(StreamReader streamReader, int recordLength, List<int> fieldLengths)
        {
            _streamReader = streamReader;
            _recordLength = recordLength;
            _fieldLengths = fieldLengths;
        }

        public bool HasMoreRecords()
        {
            return _streamReader.Peek() >= 0;
        }

        public List<string> GetNextValue()
        {
            // Read bytes according to recordLength
            int len = _recordLength;
            char[] buffer = new char[len];
            _streamReader.ReadBlock(buffer, 0, len);

            string recordString = new string(buffer);

            List<string> fields = GetFields(recordString);
            return fields;
        }

        private List<string> GetFields(string recordString)
        {
            List<string> fields = new List<string>();
            int offset = 0;
            foreach (int fieldLength in _fieldLengths)
            {
                string fieldString = recordString.Substring(offset, fieldLength);
                offset += fieldLength;
                fields.Add(fieldString);
            }
            return fields;
        }
    }
}