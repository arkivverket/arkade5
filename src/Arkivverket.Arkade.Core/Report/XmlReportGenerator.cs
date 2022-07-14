using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Core.Report
{
    public class XmlReportGenerator : IReportGenerator
    {
        public void Generate(TestReport testReport, Stream stream)
        {
            var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings{ Indent = true});
            new XmlSerializer(typeof(TestReport)).Serialize(xmlWriter, testReport);
        }
    }
}
