using System.IO;
using Arkivverket.Arkade.Report;
using Arkivverket.Arkade.Test.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Report
{
    public class HtmlToPdfConverterTest
    {
        [Fact]
        public void ShouldConvertHtmlToPdf()
        {
            string reportHtml = TestUtil.ReadFromFileInTestDataDir("Report\\report.html");
            byte[] pdf = HtmlToPdfConverter.Convert(reportHtml);
            pdf.Should().NotBeNullOrEmpty();

            //File.WriteAllBytes("c:\\tmp\\report.pdf", pdf);
        }
    }
}