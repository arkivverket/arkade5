using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class Noark4FileReader : IFlatFileReader
    {
        private readonly XmlReader _reader;

        private readonly AddmlRecordDefinition _addmlRecordDefinition;

        private Record _nextRecord;

        public Noark4FileReader(FlatFile file)
        {
            string filename = file.GetName();

            // TODO: Support for multiple RecordDefinitions per file
            _addmlRecordDefinition = file.Definition.AddmlRecordDefinitions[0];

            _reader = XmlReader.Create(filename);
            _reader.MoveToContent();
        }

        public bool HasMoreRecords()
        {
            string recordDefinitionName = _addmlRecordDefinition.Name;

            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element && _reader.Name == recordDefinitionName)
                {
                    XElement el = XNode.ReadFrom(_reader) as XElement;

                    List<Field> fields = ParseFields(el);
                    _nextRecord = new Record(_addmlRecordDefinition, fields);
                    return true;
                }
            }

            return false;
        }

        public Record GetNextRecord()
        {
            Record ret = _nextRecord;
            _nextRecord = null;
            return ret;
        }

        private List<Field> ParseFields(XElement node)
        {
            List<Field> fields = new List<Field>();

            // TODO: Use XmlFormatReader!

            return fields;
        }
    }
}