using System;
using System.IO;
using System.Xml;

namespace Arkivverket.Arkade.Core.Addml
{
    public class XmlUtil
    {
        public static XmlReader Read(StringReader s)
        {
            // Ignore DTD in XML
            XmlReaderSettings settings = new XmlReaderSettings
            {
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Parse
            };
            return XmlReader.Create(s, settings);
        }

        public static XmlReader Read(string xml)
        {
            StringReader stringReader = new StringReader(xml);
            return Read(stringReader);
        }
    }
}