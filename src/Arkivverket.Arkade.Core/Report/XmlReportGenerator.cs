using System.IO;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Core.Report
{
    public class XmlReportGenerator : IReportGenerator
    {
        public void Generate(TestReport testReport, StreamWriter stream)
        {
            var serializer = new XmlSerializer(typeof(TestReport));
            using var sw = new StringWriter();
            serializer.Serialize(sw, testReport);
            stream.Write(sw.ToString());
            stream.Flush();
        }
    }
}
