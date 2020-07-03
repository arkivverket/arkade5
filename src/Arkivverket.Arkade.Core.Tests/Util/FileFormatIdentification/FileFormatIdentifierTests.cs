using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util.FileFormatIdentification
{
    public class SiegfriedFileFormatIdentifierTests
    {
        [Fact, Trait("Category", "Integration")]
        public void IdentifyTest()
        {
            var directory = new DirectoryInfo(Path.Combine("TestData", "FileTypes")); // PDF, PDF/A, DOCX

            IFileFormatIdentifier formatIdentifier = new SiegfriedFileFormatIdentifier();

            List<SiegfriedFileInfo> filesWithFormat = formatIdentifier.IdentifyFormat(directory).ToList();

            // PDF

            SiegfriedFileInfo pdfSiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("pdf-file.pdf"));

            pdfSiegfriedFileInfo.Id.Should().Be("fmt/276");
            pdfSiegfriedFileInfo.Format.Should().Be("Acrobat PDF 1.7 - Portable Document Format");
            pdfSiegfriedFileInfo.Version.Should().Be("1.7");
            pdfSiegfriedFileInfo.Errors.Should().BeEmpty();

            // PDF/A

            SiegfriedFileInfo pdfASiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("pdfa-file.pdf"));

            pdfASiegfriedFileInfo.Id.Should().Be("fmt/479");
            pdfASiegfriedFileInfo.Format.Should().Be("Acrobat PDF/A - Portable Document Format");
            pdfASiegfriedFileInfo.Version.Should().Be("3a");
            pdfASiegfriedFileInfo.Errors.Should().BeEmpty();

            // DOCX

            SiegfriedFileInfo docXSiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("docx-file.docx"));

            docXSiegfriedFileInfo.Id.Should().Be("fmt/412");
            docXSiegfriedFileInfo.Format.Should().Be("Microsoft Word for Windows");
            docXSiegfriedFileInfo.Version.Should().Be("2007 onwards");
            docXSiegfriedFileInfo.Errors.Should().BeEmpty();
        }
    }
}
