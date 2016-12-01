using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml
{
    public class NoarkihToAddmlTransformer
    {
        public static string Transform(string noarkihXmlString)
        {
            XmlReader xsltFile = GetXslt();
            XPathDocument input = GetNoarkih(noarkihXmlString);

            StringWriter addmlWriter = new StringWriter();
            XmlTextWriter addmlOutput = new XmlTextWriter(addmlWriter);

            XslCompiledTransform xslTransform = new XslCompiledTransform();
            xslTransform.Load(xsltFile);
            xslTransform.Transform(input, null, addmlOutput);
            // TODO: Use Stream instead??
            return addmlWriter.ToString();
        }

        private static XPathDocument GetNoarkih(string noark4Xml)
        {
            XmlReader xmlReader = XmlReaderUtil.Read(noark4Xml);
            return new XPathDocument(xmlReader);
        }

        private static XmlReader GetXslt()
        {
            string xsl = ResourceUtil.ReadResource("Arkivverket.Arkade.ExternalModels.xsl.noarkih-to-addml.xsl");
            StringReader reader = new StringReader(xsl);
            return XmlReader.Create(reader);
        }
    }
}