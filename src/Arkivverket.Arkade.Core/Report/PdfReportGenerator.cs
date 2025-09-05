using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Util;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using iText.Pdfa;

namespace Arkivverket.Arkade.Core.Report
{
    public class PdfReportGenerator : IReportGenerator
    {
        private readonly int _testResultDisplayLimit;

        public PdfReportGenerator(int testResultDisplayLimit)
        {
            _testResultDisplayLimit = testResultDisplayLimit;
        }

        public void Generate(TestReport testReport, Stream stream)
        {
            using var memoryStream = new MemoryStream();
            var reportGenerator = new HtmlReportGenerator(_testResultDisplayLimit);
            reportGenerator.Generate(testReport, memoryStream);
            string htmlString = Encoding.UTF8.GetString(memoryStream.ToArray());

            var pdfADocument = new PdfADocument(
                new PdfWriter(stream),
                PdfAConformance.PDF_A_1B,
                new PdfOutputIntent(
                    "Custom", "", "http://www.color.org", "sRGB IEC61966-2.1",
                    ResourceUtil.GetResourceAsStream("Arkivverket.Arkade.Core.Resources.sRGB_CS_profile.icm")
                )
            );

            var fontProvider = new FontProvider();
            byte[] font = ResourceUtil.ReadResourceBytes("Arkivverket.Arkade.Core.Resources.FreeSans.ttf");
            fontProvider.AddFont(font);
            var converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(fontProvider);

            HtmlConverter.ConvertToPdf(htmlString, pdfADocument, converterProperties);
        }
    }
}
