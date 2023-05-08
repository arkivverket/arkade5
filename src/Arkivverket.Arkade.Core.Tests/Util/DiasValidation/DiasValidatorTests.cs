using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Tar;
using Xunit;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Tests.Util.DiasValidation
{
    public class DiasValidatorTests
    {
        private readonly DiasValidator _diasValidator = new();

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateSipDirectory()
        {
            DiasDirectory diasDirectory = DiasProvider.ProvideForFormat(ArchiveFormat.DiasSip);

            var testDirectory = new DirectoryInfo("testDirectory");

            Write(diasDirectory, testDirectory.FullName);

            _diasValidator.ValidateAsync(testDirectory, ArchiveFormat.DiasSip).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(testDirectory);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateAipDirectory()
        {
            DiasDirectory diasDirectory = DiasProvider.ProvideForFormat(ArchiveFormat.DiasAip);

            var testDirectory = new DirectoryInfo("testDirectory");

            Write(diasDirectory, testDirectory.FullName);

            _diasValidator.ValidateAsync(testDirectory, ArchiveFormat.DiasAip).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(testDirectory);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateN5SipDirectory()
        {
            DiasDirectory diasDirectory = DiasProvider.ProvideForFormat(ArchiveFormat.DiasSipN5);

            var testDirectory = new DirectoryInfo("testDirectory");

            Write(diasDirectory, testDirectory.FullName);

            _diasValidator.ValidateAsync(testDirectory, ArchiveFormat.DiasSipN5).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(testDirectory);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateN5AipDirectory()
        {
            DiasDirectory diasDirectory = DiasProvider.ProvideForFormat(ArchiveFormat.DiasAipN5);

            var testDirectory = new DirectoryInfo("testDirectory");

            Write(diasDirectory, testDirectory.FullName);

            _diasValidator.ValidateAsync(testDirectory, ArchiveFormat.DiasAipN5).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(testDirectory);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateSiardAipDirectory()
        {
            DiasDirectory diasDirectory = DiasProvider.ProvideForFormat(ArchiveFormat.DiasAipSiard);

            var testDirectory = new DirectoryInfo("testDirectory");

            Write(diasDirectory, testDirectory.FullName);

            ArchiveFormatValidationReport report = _diasValidator.ValidateAsync(testDirectory, ArchiveFormat.DiasAipSiard)
                .Result;

            report.ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(testDirectory);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateSiardSipDirectory()
        {
            DiasDirectory diasDirectory = DiasProvider.ProvideForFormat(ArchiveFormat.DiasSipSiard);

            var testDirectory = new DirectoryInfo("testDirectory");

            Write(diasDirectory, testDirectory.FullName);

            ArchiveFormatValidationReport report = _diasValidator.ValidateAsync(testDirectory, ArchiveFormat.DiasSipSiard)
                .Result;

            report.ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(testDirectory);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldInvalidateN5SipDirectory()
        {
            DiasDirectory diasDirectory = DiasProvider.ProvideForFormat(ArchiveFormat.DiasSip);

            var testDirectory = new DirectoryInfo("testDirectory");

            Write(diasDirectory, testDirectory.FullName);

            ArchiveFormatValidationReport report = _diasValidator.ValidateAsync(testDirectory, ArchiveFormat.DiasSipN5)
                .Result;

            report.ValidationResult.Should().Be(ArchiveFormatValidationResult.Invalid);
            report.ValidationSummary().Should().Contain(Path.Combine(DirectoryNameContent, ArkivstrukturXmlFileName));

            Delete(testDirectory);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateSipZipArchive()
        {
            DiasEntry[] diasEntries = DiasProvider.ProvideForFormat(ArchiveFormat.DiasSip).GetEntries();

            FileInfo diasTarArchive = CreateDiasTarArchive("testArchive.tar", diasEntries);

            _diasValidator.ValidateAsync(diasTarArchive, ArchiveFormat.DiasSip).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(diasTarArchive);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateAipZipArchive()
        {
            DiasEntry[] diasEntries = DiasProvider.ProvideForFormat(ArchiveFormat.DiasAip).GetEntries();

            FileInfo diasTarArchive = CreateDiasTarArchive("testArchive.tar", diasEntries);

            _diasValidator.ValidateAsync(diasTarArchive, ArchiveFormat.DiasAip).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(diasTarArchive);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateN5SipZipArchive()
        {
            DiasEntry[] diasEntries = DiasProvider.ProvideForFormat(ArchiveFormat.DiasSipN5).GetEntries();

            FileInfo diasTarArchive = CreateDiasTarArchive("testArchive.tar", diasEntries);

            _diasValidator.ValidateAsync(diasTarArchive, ArchiveFormat.DiasSipN5).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(diasTarArchive);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldValidateN5AipZipArchive()
        {
            DiasEntry[] diasEntries = DiasProvider.ProvideForFormat(ArchiveFormat.DiasAipN5).GetEntries();

            FileInfo diasTarArchive = CreateDiasTarArchive("testArchive.tar", diasEntries);

            _diasValidator.ValidateAsync(diasTarArchive, ArchiveFormat.DiasAipN5).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(diasTarArchive);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldInvalidateN5SipZipArchive()
        {
            DiasEntry[] diasEntries = DiasProvider.ProvideForFormat(ArchiveFormat.DiasAip).GetEntries();

            FileInfo diasTarArchive = CreateDiasTarArchive("testArchive.tar", diasEntries);

            ArchiveFormatValidationReport report =
                _diasValidator.ValidateAsync(diasTarArchive, ArchiveFormat.DiasSipN5).Result;

            report.ValidationResult.Should().Be(ArchiveFormatValidationResult.Invalid);
            report.ValidationSummary().Should().Contain(Path.Combine(DirectoryNameContent, ArkivstrukturXmlFileName));

            Delete(diasTarArchive);
        }

        [Fact]
        [Trait("Dependency", "IO")]
        public void ShouldReportInvalidButAcceptableDias()
        {
            DiasDirectory diasSource = DiasProvider.ProvideForFormat(ArchiveFormat.DiasSipN5);
            diasSource.DeleteEntry(ChangeLogXmlFileName, true);

            DiasEntry[] diasEntries = diasSource.GetEntries();
            FileInfo diasTarArchive = CreateDiasTarArchive("testArchive.tar", diasEntries);

            ArchiveFormatValidationReport report =
                _diasValidator.ValidateAsync(diasTarArchive, ArchiveFormat.DiasSipN5).Result;

            report.ValidationResult.Should().Be(ArchiveFormatValidationResult.Invalid);
            report.IsAcceptable.Should().BeTrue();
            report.ValidationSummary().Should().Contain(Path.Combine(DirectoryNameContent, ChangeLogXmlFileName));

            Delete(diasTarArchive);
        }

        private static FileInfo CreateDiasTarArchive(string fileName, IEnumerable<DiasEntry> diasEntries)
        {
            var outputStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            using var tarOutputStream = new TarOutputStream(outputStream, Encoding.Latin1);

            CreateDiasTarArchiveEntries(diasEntries, Path.GetFileNameWithoutExtension(fileName), tarOutputStream);

            return new FileInfo(fileName);
        }

        private static void CreateDiasTarArchiveEntries(IEnumerable<DiasEntry> diasEntries, string path, TarOutputStream archive)
        {
            foreach (DiasEntry diasEntry in diasEntries)
            {
                var tarEntry = TarEntry.CreateTarEntry(Path.Join(path, diasEntry.Name));

                if (diasEntry is DiasDirectory diasSubDirectory)
                {
                    tarEntry.TarHeader.TypeFlag = TarHeader.LF_DIR;

                    string diasSubDirectoryPath = Path.Join(path, diasSubDirectory.Name);

                    CreateDiasTarArchiveEntries(diasSubDirectory.GetEntries(), diasSubDirectoryPath, archive);
                }

                archive.PutNextEntry(tarEntry);
                archive.CloseEntry();
            }
        }

        private static void Write(DiasDirectory diasDirectory, string directoryName)
        {
            WriteEntries(diasDirectory.GetEntries(), Directory.CreateDirectory(directoryName).FullName);
        }

        private static void WriteEntries(IEnumerable<DiasEntry> entries, string path)
        {
            foreach (DiasEntry entry in entries)
            {
                string entryName = Path.Join(path, entry.Name);

                if (entry is DiasFile)
                    using (File.Create(entryName)) { } //This is needed to release the resource before cleanup

                if (entry is DiasDirectory diasDirectory)
                {
                    DirectoryInfo directory = Directory.CreateDirectory(entryName);
                    WriteEntries(diasDirectory.GetEntries(), directory.FullName);
                }
            }
        }

        private static void Delete(FileSystemInfo item)
        {
            if (item is DirectoryInfo directory)
                directory.Delete(recursive: true);

            else item.Delete();

            if (item.Exists)
                throw new Exception("Unable to delete: " + item);
        }
    }
}
