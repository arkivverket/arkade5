using System;
using System.IO;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Core.Report
{
    public class XmlReportSchemaGenerator : IReportSchemaGenerator
    {
        public void Generate(Type reportType, TextWriter schemaWriter)
        {
            var xmlSchemas = new XmlSchemas();
            var exporter = new XmlSchemaExporter(xmlSchemas);
            XmlTypeMapping mapping = new XmlReflectionImporter().ImportTypeMapping(reportType);
            exporter.ExportTypeMapping(mapping);
            foreach (XmlSchema xmlSchema in xmlSchemas)
                xmlSchema.Write(schemaWriter);
        }
    }
}
