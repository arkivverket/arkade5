namespace Arkivverket.Arkade.Report
{
    public class PdfReportGenerator : IReportGenerator<PdfReport>
    {
        private readonly HtmlReportGenerator _htmlReportGenerator = new HtmlReportGenerator();

        public PdfReportGenerator()
        {
        }

        public PdfReport Generate()
        {
            HtmlReport htmlReport = _htmlReportGenerator.Generate();
            byte[] pdf = HtmlToPdfConverter.Convert(htmlReport.GetHtml());
            return new PdfReport(pdf);
        }
    }
}