using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util.FileFormatIdentification
{
    public class SiegfriedFileInfoTests
    {
        [Fact]
        public void CreateFromStringTest()
        {
            const string siegfriedAnalysisResult = "dokumenter\\5000000.pdf,"
                                                   + "20637,2016-02-12T15:57:18+01:00,"
                                                   + "Some Siegfried error message,"
                                                   + "pronom,fmt/18,Acrobat PDF 1.4 - Portable Document Format,"
                                                   + "1.4,application/pdf,"
                                                   + "extension match pdf; byte match at [[0 8] [20631 5]],";

            IFileFormatInfo siegfriedFileInfo = SiegfriedFileInfo.CreateFromString(siegfriedAnalysisResult);

            siegfriedFileInfo.FileName.Should().Be("dokumenter\\5000000.pdf");
            siegfriedFileInfo.FileExtension.Should().Be(".pdf");
            siegfriedFileInfo.Id.Should().Be("fmt/18");
            siegfriedFileInfo.Format.Should().Be("Acrobat PDF 1.4 - Portable Document Format");
            siegfriedFileInfo.Version.Should().Be("1.4");
            siegfriedFileInfo.MimeType.Should().Be("application/pdf");
            siegfriedFileInfo.ByteSize.Should().Be("20637");
            siegfriedFileInfo.Errors.Should().Be("Some Siegfried error message");
        }

        [Fact]
        public void CreateFromStringTestWithBadData()
        {
            const string siegfriedAnalysisResult = "dokumenter\\5000000.pdf,"
                                                   + "20637,2016-02-12T15:57:18+01:00,"
                                                   + "Some Siegfried error message,"
                                                   + "pronom,fmt/18,Acrobat PDF 1.4 - Portable Document Format,"
                                                   + "1.4,appli\"cation/pdf," // Contains single quote character
                                                   + "extension match pdf; byte match at [[0 8] [20631 5]],";

            IFileFormatInfo siegfriedFileInfo = SiegfriedFileInfo.CreateFromString(siegfriedAnalysisResult);

            siegfriedFileInfo.FileName.Should().Be("dokumenter\\5000000.pdf");
            siegfriedFileInfo.FileExtension.Should().Be(".pdf");
            siegfriedFileInfo.Id.Should().Be("fmt/18");
            siegfriedFileInfo.Format.Should().Be("Acrobat PDF 1.4 - Portable Document Format");
            siegfriedFileInfo.Version.Should().Be("1.4");
            siegfriedFileInfo.MimeType.Should().Be("appli\"cation/pdf");
            siegfriedFileInfo.ByteSize.Should().Be("20637");
            siegfriedFileInfo.Errors.Should().Be("Some Siegfried error message, Analysis result parse problem");
        }
    }
}
