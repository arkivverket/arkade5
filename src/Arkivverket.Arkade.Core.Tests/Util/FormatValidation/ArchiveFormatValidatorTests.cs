using System;
using System.IO;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util.FormatValidation
{
    public class ArchiveFormatValidatorTests
    {
        private static readonly string TestFilesDirectoryPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "FileTypes");

        private readonly FileInfo _approvedPdfAFile = new(Path.Combine(TestFilesDirectoryPath, "pdfA-1b.pdf"));
        private readonly FileInfo _disapprovedPdfAFile = new(Path.Combine(TestFilesDirectoryPath, "pdfA-3a.pdf"));
        private readonly FileInfo _regularPdfFile = new(Path.Combine(TestFilesDirectoryPath, "pdf.pdf"));
        private readonly FileInfo _docxFile = new(Path.Combine(TestFilesDirectoryPath, "docx.docx"));
        private readonly FileInfo _nonExistingFile = new("invalid_filepath");

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Dependency", "JRE")]
        public void ValidateAsFormatTest()
        {
            ArchiveFormatValidationReport validationReport =
                ArchiveFormatValidator.ValidateAsFormat(_approvedPdfAFile, ArchiveFormat.PdfA).Result.GetReport();

            validationReport.ValidationFormat.Should().Be(ArchiveFormat.PdfA);
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Dependency", "JRE")]
        public void ValidateAsPdfATest()
        {
            ArchiveFormatValidator.ValidateAsPdfA(_approvedPdfAFile).Result.GetReport()
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            ArchiveFormatValidator.ValidateAsPdfA(_disapprovedPdfAFile).Result.GetReport()
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Invalid);

            ArchiveFormatValidator.ValidateAsPdfA(_regularPdfFile).Result.GetReport()
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Invalid);

            ArchiveFormatValidator.ValidateAsPdfA(_docxFile).Result.GetReport()
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Error);

            ArchiveFormatValidator.ValidateAsPdfA(_nonExistingFile).Result.GetReport()
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Error);
        }
    }
}
