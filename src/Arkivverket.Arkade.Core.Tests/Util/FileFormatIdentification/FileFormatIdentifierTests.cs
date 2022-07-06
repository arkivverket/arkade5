using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Logging;
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
            var directoryPath = Path.Combine("TestData", "FileTypes"); // PDF, PDF/A-1b, PDF/A-3a, DOCX

            IFileFormatIdentifier formatIdentifier = CreateFileFormatIdentifier();

            List<IFileFormatInfo> filesWithFormat = formatIdentifier.IdentifyFormats(directoryPath, FileFormatScanMode.Directory).ToList();

            // PDF

            IFileFormatInfo pdfSiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("pdf.pdf"));

            pdfSiegfriedFileInfo.Id.Should().Be("fmt/276");
            pdfSiegfriedFileInfo.Format.Should().Be("Acrobat PDF 1.7 - Portable Document Format");
            pdfSiegfriedFileInfo.Version.Should().Be("1.7");
            pdfSiegfriedFileInfo.Errors.Should().BeEmpty();

            // PDF/A-1B

            IFileFormatInfo pdfA1bSiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("pdfA-1b.pdf"));

            pdfA1bSiegfriedFileInfo.Id.Should().Be("fmt/354");
            pdfA1bSiegfriedFileInfo.Format.Should().Be("Acrobat PDF/A - Portable Document Format");
            pdfA1bSiegfriedFileInfo.Version.Should().Be("1b");
            pdfA1bSiegfriedFileInfo.Errors.Should().BeEmpty();
            
            // PDF/A-3A (disapproved PDF/A variant)

            IFileFormatInfo pdfA3aSiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("pdfA-3a.pdf"));

            pdfA3aSiegfriedFileInfo.Id.Should().Be("fmt/479");
            pdfA3aSiegfriedFileInfo.Format.Should().Be("Acrobat PDF/A - Portable Document Format");
            pdfA3aSiegfriedFileInfo.Version.Should().Be("3a");
            pdfA3aSiegfriedFileInfo.Errors.Should().BeEmpty();

            // DOCX

            IFileFormatInfo docXSiegfriedFileInfo = filesWithFormat.First(f => f.FileName.EndsWith("docx.docx"));

            docXSiegfriedFileInfo.Id.Should().Be("fmt/412");
            docXSiegfriedFileInfo.Format.Should().Be("Microsoft Word for Windows");
            docXSiegfriedFileInfo.Version.Should().Be("2007 onwards");
            docXSiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifySingleDocxFileTest()
        {
            IFileFormatIdentifier formatIdentifier = CreateFileFormatIdentifier();

            string docxFilePath = Path.Combine("TestData", "FileTypes", "docx.docx");

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
            IFileFormatIdentifier formatIdentifier = CreateFileFormatIdentifier();

            string docxFilePath = Path.Combine("TestData", "FileTypes", "docx.docx");

            var bytes = File.ReadAllBytes(docxFilePath);

            var target = new KeyValuePair<string, IEnumerable<byte>>(docxFilePath, bytes);

            IFileFormatInfo docXSiegfriedFileInfo = formatIdentifier.IdentifyFormat(target);

            docXSiegfriedFileInfo.Id.Should().Be("fmt/412");
            docXSiegfriedFileInfo.Format.Should().Be("Microsoft Word for Windows");
            docXSiegfriedFileInfo.Version.Should().Be("2007 onwards");
            docXSiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifySinglePdfAFileTest()
        {
            IFileFormatIdentifier formatIdentifier = CreateFileFormatIdentifier();

            string pdfaFilePath = Path.Combine("TestData", "FileTypes", "pdfA-1b.pdf");

            var pdfaFile = new FileInfo(pdfaFilePath);

            IFileFormatInfo pdfASiegfriedFileInfo = formatIdentifier.IdentifyFormat(pdfaFile);

            pdfASiegfriedFileInfo.Id.Should().Be("fmt/354");
            pdfASiegfriedFileInfo.Format.Should().Be("Acrobat PDF/A - Portable Document Format");
            pdfASiegfriedFileInfo.Version.Should().Be("1b");
            pdfASiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifyPdfAFileFromFileStreamTest()
        {
            IFileFormatIdentifier formatIdentifier = CreateFileFormatIdentifier();

            string pdfaFilePath = Path.Combine("TestData", "FileTypes", "pdfA-1b.pdf");

            var bytes = File.ReadAllBytes(pdfaFilePath);
            
            var target = new KeyValuePair<string, IEnumerable<byte>>(pdfaFilePath, bytes);

            IFileFormatInfo pdfASiegfriedFileInfo = formatIdentifier.IdentifyFormat(target);

            pdfASiegfriedFileInfo.Id.Should().Be("fmt/354");
            pdfASiegfriedFileInfo.Format.Should().Be("Acrobat PDF/A - Portable Document Format");
            pdfASiegfriedFileInfo.Version.Should().Be("1b");
            pdfASiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IdentifySinglePdfFileTest()
        {
            IFileFormatIdentifier formatIdentifier = CreateFileFormatIdentifier();

            string pdfFilePath = Path.Combine("TestData", "FileTypes", "pdf.pdf");

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
            IFileFormatIdentifier formatIdentifier = CreateFileFormatIdentifier();

            string pdfFilePath = Path.Combine("TestData", "FileTypes", "pdf.pdf");

            var bytes = File.ReadAllBytes(pdfFilePath);

            var target = new KeyValuePair<string, IEnumerable<byte>>(pdfFilePath, bytes);

            IFileFormatInfo pdfSiegfriedFileInfo = formatIdentifier.IdentifyFormat(target);

            pdfSiegfriedFileInfo.Id.Should().Be("fmt/276");
            pdfSiegfriedFileInfo.Format.Should().Be("Acrobat PDF 1.7 - Portable Document Format");
            pdfSiegfriedFileInfo.Version.Should().Be("1.7");
            pdfSiegfriedFileInfo.Errors.Should().BeEmpty();
        }

        private static IFileFormatIdentifier CreateFileFormatIdentifier()
        {
            return new SiegfriedFileFormatIdentifier(new SiegfriedProcessRunner(new StatusEventHandler()));
        }
    }
}
