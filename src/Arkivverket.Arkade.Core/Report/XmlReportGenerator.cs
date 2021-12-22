using System.IO;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Core.Report
{
    public class XmlReportGenerator : IReportGenerator
    {
        public void Generate(TestReport testReport, Stream stream)
        {
            new XmlSerializer(typeof(TestReport)).Serialize(stream, testReport);
        }
    }
}
