using System;
using System.IO;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util.FormatValidation
{
    public class ArchiveFormatValidatorTests : LanguageDependentTest, IClassFixture<ArchiveFormatValidatorFixture>
    {
        private static readonly string TestFilesDirectoryPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "FileTypes");

        private readonly FileInfo _approvedPdfAFile = new(Path.Combine(TestFilesDirectoryPath, "pdfA-1b.pdf"));
        private readonly FileInfo _disapprovedPdfAFile = new(Path.Combine(TestFilesDirectoryPath, "pdfA-3a.pdf"));
        private readonly FileInfo _regularPdfFile = new(Path.Combine(TestFilesDirectoryPath, "pdf.pdf"));
        private readonly FileInfo _docxFile = new(Path.Combine(TestFilesDirectoryPath, "docx.docx"));
        private readonly FileInfo _nonExistingFile = new("invalid_filepath");

        private readonly ArchiveFormatValidatorFixture _fixture;

        public ArchiveFormatValidatorTests(ArchiveFormatValidatorFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Dependency", "JRE")]
        public void ValidateAsFormatTest()
        {
            ArchiveFormatValidationReport validationReport =
                _fixture.ArchiveFormatValidator.ValidateAsync(_approvedPdfAFile, ArchiveFormat.PdfA).Result;

            validationReport.ValidationFormat.Should().Be(ArchiveFormat.PdfA);
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Dependency", "JRE")]
        public void PdfA1BShouldBeValidPdfAFormat()
        {
            ArchiveFormatValidationReport validationReport = 
                _fixture.ArchiveFormatValidator.ValidateAsync(_approvedPdfAFile, ArchiveFormat.PdfA).Result;

            validationReport.ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Dependency", "JRE")]
        public void PdfA3AShouldBeInvalidPdfAFormat()
        {
            ArchiveFormatValidationReport archiveFormatValidationReport = 
                _fixture.ArchiveFormatValidator.ValidateAsync(_disapprovedPdfAFile, ArchiveFormat.PdfA).Result;

            archiveFormatValidationReport.ValidationResult.Should().Be(ArchiveFormatValidationResult.Invalid);
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Dependency", "JRE")]
        public void PdfShouldBeInvalidPdfAFormat()
        {
            ArchiveFormatValidationReport archiveFormatValidationReport = 
                _fixture.ArchiveFormatValidator.ValidateAsync(_regularPdfFile, ArchiveFormat.PdfA).Result;

            archiveFormatValidationReport.ValidationResult.Should().Be(ArchiveFormatValidationResult.Invalid);
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Dependency", "JRE")]
        public void DocXShouldBeReportedAsErrorForPdfAValidation()
        {
            ArchiveFormatValidationReport archiveFormatValidationReport =
                _fixture.ArchiveFormatValidator.ValidateAsync(_docxFile, ArchiveFormat.PdfA).Result;

            archiveFormatValidationReport.ValidationResult.Should().Be(ArchiveFormatValidationResult.Error);
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Dependency", "JRE")]
        public void NonExistingFileShouldBeReportedAsErrorForPdfAValidation()
        {
            ArchiveFormatValidationReport archiveFormatValidationReport =
                _fixture.ArchiveFormatValidator.ValidateAsync(_nonExistingFile, ArchiveFormat.PdfA).Result;

            archiveFormatValidationReport.ValidationResult.Should().Be(ArchiveFormatValidationResult.Error);
            archiveFormatValidationReport.ValidationInfo.Should().Be($"Valideringen mislyktes: '{_nonExistingFile.FullName}' ble ikke funnet.");
        }
    }

    public class ArchiveFormatValidatorFixture : IDisposable
    {
        public readonly IArchiveFormatValidator ArchiveFormatValidator;

        public ArchiveFormatValidatorFixture()
        {
            ArchiveFormatValidator = new ArchiveFormatValidator();
        }

        public void Dispose()
        {
            ArchiveFormatValidator.Dispose();
        }
    }
}
