using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Arkivverket.Arkade.Core.Addml
{
    public class XmlFormatReader
    {
        private readonly XmlReader _reader;

        private readonly string _recordName;

        private Dictionary<string, string> _nextFieldKeyValues;

        public XmlFormatReader(XmlReader reader, string recordName)
        {
            _reader = reader;
            _recordName = recordName;
            _reader.MoveToContent();
        }

        public bool HasNext()
        {
            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element && _reader.Name == _recordName)
                {
                    XmlReader inner = _reader.ReadSubtree();
                    _nextFieldKeyValues = ParseInner(inner);
                    return true;
                }
            }

            return false;
        }

        private Dictionary<string, string> ParseInner(XmlReader inner)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            while (inner.Read())
            {
                if (inner.NodeType == XmlNodeType.Element && inner.Name != _recordName)
                {
                    XElement element = XElement.Load(inner.ReadSubtree());
                    string name = element.Name.LocalName;
                    string value = element.Value;
                    fields.Add(name, value);
                }
            }
            return fields;
        }

        public Dictionary<string, string> Next()
        {
            if (_nextFieldKeyValues == null)
            {
                throw new InvalidOperationException(
                    "HasNext() must be called before Next()");
            }

            Dictionary<string, string> ret = _nextFieldKeyValues;
            _nextFieldKeyValues = null;
            return ret;
        }
    }
}