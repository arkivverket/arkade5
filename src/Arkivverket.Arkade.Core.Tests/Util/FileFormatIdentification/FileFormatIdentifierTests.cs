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

            List<IFileFormatInfo> filesWithFormat = formatIdentifier.IdentifyFormat(directory).ToList();

            // PDF

            IFileFormatInfo pdfSiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("pdf-file.pdf"));

            pdfSiegfriedFileInfo.Id.Should().Be("fmt/276");
            pdfSiegfriedFileInfo.Format.Should().Be("Acrobat PDF 1.7 - Portable Document Format");
            pdfSiegfriedFileInfo.Version.Should().Be("1.7");
            pdfSiegfriedFileInfo.Errors.Should().BeEmpty();

            // PDF/A

            IFileFormatInfo pdfASiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("pdfa-file.pdf"));

            pdfASiegfriedFileInfo.Id.Should().Be("fmt/479");
            pdfASiegfriedFileInfo.Format.Should().Be("Acrobat PDF/A - Portable Document Format");
            pdfASiegfriedFileInfo.Version.Should().Be("3a");
            pdfASiegfriedFileInfo.Errors.Should().BeEmpty();

            // DOCX

            IFileFormatInfo docXSiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("docx-file.docx"));

            docXSiegfriedFileInfo.Id.Should().Be("fmt/412");
            docXSiegfriedFileInfo.Format.Should().Be("Microsoft Word for Windows");
            docXSiegfriedFileInfo.Version.Should().Be("2007 onwards");
            docXSiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifySingleDocxFileTest()
        {
            IFileFormatIdentifier formatIdentifier = new SiegfriedFileFormatIdentifier();

            string docxFilePath = Path.Combine("TestData", "FileTypes", "docx-file.docx");

            var docxFile = new FileInfo(docxFilePath);

            IFileFormatInfo docXSiegfriedFileInfo = formatIdentifier.IdentifyFormat(docxFile);

            docXSiegfriedFileInfo.Id.Should().Be("fmt/412");
            docXSiegfriedFileInfo.Format.Should().Be("Microsoft Word for Windows");
            docXSiegfriedFileInfo.Version.Should().Be("2007 onwards");
            docXSiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifyDocxFileFromFileStreamTest()
        {
            IFileFormatIdentifier formatIdentifier = new SiegfriedFileFormatIdentifier();

            string docxFilePath = Path.Combine("TestData", "FileTypes", "docx-file.docx");

            var fileStream = new FileStream(docxFilePath, FileMode.Open, FileAccess.Read);

            var target = new KeyValuePair<string, Stream>(docxFilePath, fileStream);

            IFileFormatInfo docXSiegfriedFileInfo = formatIdentifier.IdentifyFormat(target);

            docXSiegfriedFileInfo.Id.Should().Be("fmt/412");
            docXSiegfriedFileInfo.Format.Should().Be("Microsoft Word for Windows");
            docXSiegfriedFileInfo.Version.Should().Be("2007 onwards");
            docXSiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifySinglePdfAFileTest()
        {
            IFileFormatIdentifier formatIdentifier = new SiegfriedFileFormatIdentifier();

            string pdfaFilePath = Path.Combine("TestData", "FileTypes", "pdfa-file.pdf");

            var pdfaFile = new FileInfo(pdfaFilePath);

            IFileFormatInfo pdfASiegfriedFileInfo = formatIdentifier.IdentifyFormat(pdfaFile);

            pdfASiegfriedFileInfo.Id.Should().Be("fmt/479");
            pdfASiegfriedFileInfo.Format.Should().Be("Acrobat PDF/A - Portable Document Format");
            pdfASiegfriedFileInfo.Version.Should().Be("3a");
            pdfASiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifyPdfAFileFromFileStreamTest()
        {
            IFileFormatIdentifier formatIdentifier = new SiegfriedFileFormatIdentifier();

            string pdfaFilePath = Path.Combine("TestData", "FileTypes", "pdfa-file.pdf");

            var fileStream = new FileStream(pdfaFilePath, FileMode.Open, FileAccess.Read);

            var target = new KeyValuePair<string, Stream>(pdfaFilePath, fileStream);

            IFileFormatInfo pdfASiegfriedFileInfo = formatIdentifier.IdentifyFormat(target);

            pdfASiegfriedFileInfo.Id.Should().Be("fmt/479");
            pdfASiegfriedFileInfo.Format.Should().Be("Acrobat PDF/A - Portable Document Format");
            pdfASiegfriedFileInfo.Version.Should().Be("3a");
            pdfASiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifySinglePdfFileTest()
        {
            IFileFormatIdentifier formatIdentifier = new SiegfriedFileFormatIdentifier();

            string pdfFilePath = Path.Combine("TestData", "FileTypes", "pdf-file.pdf");

            var pdfFile = new FileInfo(pdfFilePath);

            IFileFormatInfo pdfSiegfriedFileInfo = formatIdentifier.IdentifyFormat(pdfFile);

            pdfSiegfriedFileInfo.Id.Should().Be("fmt/276");
            pdfSiegfriedFileInfo.Format.Should().Be("Acrobat PDF 1.7 - Portable Document Format");
            pdfSiegfriedFileInfo.Version.Should().Be("1.7");
            pdfSiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifyPdfFileFromFileStreamTest()
        {
            IFileFormatIdentifier formatIdentifier = new SiegfriedFileFormatIdentifier();

            string pdfFilePath = Path.Combine("TestData", "FileTypes", "pdf-file.pdf");

            var fileStream = new FileStream(pdfFilePath, FileMode.Open, FileAccess.Read);

            var target = new KeyValuePair<string, Stream>(pdfFilePath, fileStream);

            IFileFormatInfo pdfSiegfriedFileInfo = formatIdentifier.IdentifyFormat(target);

            pdfSiegfriedFileInfo.Id.Should().Be("fmt/276");
            pdfSiegfriedFileInfo.Format.Should().Be("Acrobat PDF 1.7 - Portable Document Format");
            pdfSiegfriedFileInfo.Version.Should().Be("1.7");
            pdfSiegfriedFileInfo.Errors.Should().BeEmpty();
        }
    }
}
