using System.IO;
using System.Text;
using iText.Html2pdf;

namespace Arkivverket.Arkade.Core.Report
{
    public class PdfReportGenerator : IReportGenerator
    {
        public void Generate(TestReport testReport, Stream stream)
        {
            using var memoryStream = new MemoryStream();
            var reportGenerator = new HtmlReportGenerator();
            reportGenerator.Generate(testReport, memoryStream);
            string htmlString = Encoding.UTF8.GetString(memoryStream.ToArray());
            HtmlConverter.ConvertToPdf(htmlString, stream);
        }
    }
}
