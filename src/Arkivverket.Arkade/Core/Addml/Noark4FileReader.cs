using System.Collections.Generic;
using System.Xml;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class Noark4FileReader : IFlatFileReader
    {
        private readonly XmlFormatReader _reader;

        private readonly AddmlRecordDefinition _addmlRecordDefinition;

        public Noark4FileReader(FlatFile file)
        {
            string filename = file.GetName();

            // TODO: Support for multiple RecordDefinitions per file
            _addmlRecordDefinition = file.Definition.AddmlRecordDefinitions[0];

            XmlReader xmlReader = XmlReader.Create(filename);
            string recordName = _addmlRecordDefinition.Name;

            _reader = new XmlFormatReader(xmlReader, recordName);
        }

        public bool HasMoreRecords()
        {
            return _reader.HasNext();
        }

        public Record GetNextRecord()
        {
            Dictionary<string, string> fieldValues = _reader.Next();

            List<Field> fields = new List<Field>();
            foreach (AddmlFieldDefinition addmlFieldDefinition in _addmlRecordDefinition.AddmlFieldDefinitions)
            {
                string fieldName = addmlFieldDefinition.Name;
                string fieldValue = fieldValues[fieldName];

                fields.Add(new Field(addmlFieldDefinition, fieldValue));
            }

            return new Record(_addmlRecordDefinition, fields);
        }
    }
}