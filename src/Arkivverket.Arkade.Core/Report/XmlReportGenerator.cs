using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Core.Report
{
    public class XmlReportGenerator : IReportGenerator
    {
        public void Generate(TestReport testReport, Stream stream)
        {
            var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings{ Indent = true, Encoding = Encoding.UTF8});
            new XmlSerializer(typeof(TestReport)).Serialize(xmlWriter, testReport);
        }
    }
}
