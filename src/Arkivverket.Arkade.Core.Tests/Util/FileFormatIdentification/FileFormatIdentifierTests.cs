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
            string testFilesDirectory = Path.Combine("TestData", "FileTypes");

            var pdfFile = new FileInfo(Path.Combine(testFilesDirectory, "pdf-file.pdf")); // PDF
            var pdfAFile = new FileInfo(Path.Combine(testFilesDirectory, "pdfa-file.pdf")); // PDF/A
            var docXFile = new FileInfo(Path.Combine(testFilesDirectory, "docx-file.docx")); // DOCX

            var files = new List<FileInfo> {pdfFile, pdfAFile, docXFile};

            IFileFormatIdentifier formatIdentifier = new SiegfriedFileFormatIdentifier();

            Dictionary<FileInfo, FileFormat> filesWithFormat = formatIdentifier.IdentifyFormat(files);

            // PDF

            FileFormat pdfFileFormat = filesWithFormat.First(f => f.Key.Name.Equals("pdf-file.pdf")).Value;

            pdfFileFormat.PuId.Should().Be("fmt/276");
            pdfFileFormat.Name.Should().Be("Acrobat PDF 1.7 - Portable Document Format");
            pdfFileFormat.Version.Should().Be("1.7");
            pdfFileFormat.MimeType.Should().Be("application/pdf");

            // PDF/A

            FileFormat pdfAFileFormat = filesWithFormat.First(f => f.Key.Name.Equals("pdfa-file.pdf")).Value;

            pdfAFileFormat.PuId.Should().Be("fmt/479");
            pdfAFileFormat.Name.Should().Be("Acrobat PDF/A - Portable Document Format");
            pdfAFileFormat.Version.Should().Be("3a");
            pdfAFileFormat.MimeType.Should().Be("application/pdf");

            // DOCX

            FileFormat docXFileFormat = filesWithFormat.First(f => f.Key.Name.Equals("docx-file.docx")).Value;

            docXFileFormat.PuId.Should().Be("fmt/412");
            docXFileFormat.Name.Should().Be("Microsoft Word for Windows");
            docXFileFormat.Version.Should().Be("2007 onwards");
            docXFileFormat.MimeType.Should().Be("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
    }
}
