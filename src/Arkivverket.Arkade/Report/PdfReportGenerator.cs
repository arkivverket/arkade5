using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Report
{
    public class PdfReportGenerator : IReportGenerator<PdfReport>
    {
        private readonly HtmlReportGenerator _htmlReportGenerator = new HtmlReportGenerator();

        public PdfReportGenerator()
        {
        }

        public PdfReport Generate(TestSession testSession)
        {
            HtmlReport htmlReport = _htmlReportGenerator.Generate(testSession);
            byte[] pdf = HtmlToPdfConverter.Convert(htmlReport.GetHtml());
            return new PdfReport(pdf);
        }
    }
}