using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml
{
    public class Noark4FileReader : IRecordEnumerator
    {
        private readonly AddmlRecordDefinition _addmlRecordDefinition;
        private readonly XmlFormatReader _reader;
        private Record _currentRecord;

        public Noark4FileReader(FlatFile file)
        {
            FileStream fileStream = file.Definition.FileInfo.OpenRead();
            XmlReader xmlReader = XmlReaderUtil.Read(fileStream);

            // TODO: Support for multiple RecordDefinitions per file
            _addmlRecordDefinition = file.Definition.AddmlRecordDefinitions[0];
            string recordName = _addmlRecordDefinition.Name;

            _reader = new XmlFormatReader(xmlReader, recordName);
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

        public Record Current => _currentRecord;

        object IEnumerator.Current => Current;

        private bool HasMoreRecords()
        {
            return _reader.HasNext();
        }

        private Record GetNextRecord()
        {
            Dictionary<string, string> fieldValues = _reader.Next();

            List<Field> fields = new List<Field>();
            foreach (AddmlFieldDefinition addmlFieldDefinition in _addmlRecordDefinition.AddmlFieldDefinitions)
            {
                string fieldName = addmlFieldDefinition.Name;

                if (fieldValues.ContainsKey(fieldName))
                {
                    string fieldValue = fieldValues[fieldName];
                    fields.Add(new Field(addmlFieldDefinition, fieldValue));
                }
                else
                {
                    // TODO: What should we do if field is missing from data?
                }
            }

            return new Record(_addmlRecordDefinition, fields);
        }
    }
}