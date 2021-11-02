using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Arkivverket.Arkade.Core.Report
{
    public class XmlReportGenerator : IReportGenerator
    {
        public void Generate(TestReport testReport, Stream stream)
        {
            // Go via JsonReportGenerator which is set up to skip fields with null values:
            using var memoryStream = new MemoryStream();
            var reportGenerator = new JsonReportGenerator();
            reportGenerator.Generate(testReport, memoryStream);
            string jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());
            testReport = JsonConvert.DeserializeObject<TestReport>(jsonString);

            var serializer = new XmlSerializer(typeof(TestReport));
            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, testReport);
            using var streamWriter = new StreamWriter(stream);
            streamWriter.Write(stringWriter.ToString());
            streamWriter.Flush();
        }
    }
}
