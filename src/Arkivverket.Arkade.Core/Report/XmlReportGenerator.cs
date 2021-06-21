using System.IO;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Core.Report
{
    public class XmlReportGenerator : IReportGenerator
    {
        public void Generate(TestReport testReport, Stream stream)
        {
            var serializer = new XmlSerializer(typeof(TestReport));
            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, testReport);
            using var streamWriter = new StreamWriter(stream);
            streamWriter.Write(stringWriter.ToString());
            streamWriter.Flush();
        }
    }
}
