using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Tar;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util.DiasValidation
{
    public class DiasValidatorTests
    {
        [Fact]
        public void DiasStructureValidator_ShouldValidateN5SIPDirectory()
        {
            const ArchiveFormat format = ArchiveFormat.DiasSipN5;

            DiasDirectory diasDirectory = DiasProvider.ProvideForFormat(format);

            var testDirectory = new DirectoryInfo("testDirectory");

            DiasProvider.Write(diasDirectory, testDirectory.FullName);

            ArchiveFormatValidator.ValidateAsFormat(testDirectory, format).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(testDirectory);
        }

        [Fact]
        public void DiasStructureValidator_ShouldInValidateN5SIPDirectory()
        {
            const ArchiveFormat format = ArchiveFormat.DiasSipN5;

            DiasEntry[] diasEntries = DiasProvider.ProvideForFormat(format).GetEntries();

            DiasEntry[] excludedDiasEntries = { diasEntries[0], diasEntries[1] };
            DiasEntry[] invalidDiasEntrySet = diasEntries.Except(excludedDiasEntries).ToArray();
            IEnumerable<string> namesOfExcludedEntries = excludedDiasEntries.Select(e => e.Name);

            var testDirectory = new DirectoryInfo("testDirectory");

            DiasProvider.Write(new DiasDirectory(string.Empty, invalidDiasEntrySet), testDirectory.FullName);

            ArchiveFormatValidationReport validationReport =
                ArchiveFormatValidator.ValidateAsFormat(testDirectory, format).Result;

            validationReport.ValidationResult.Should().Be(ArchiveFormatValidationResult.Invalid);
            validationReport.ValidationSummary().Should().ContainAll(namesOfExcludedEntries);

            Delete(testDirectory);
        }

        [Fact]
        public void DiasStructureValidator_ShouldValidateN5SIPZipArchive()
        {
            const ArchiveFormat format = ArchiveFormat.DiasSipN5;

            DiasEntry[] diasEntries = DiasProvider.ProvideForFormat(format).GetEntries();

            FileInfo diasTarArchive = CreateDiasTarArchive("testArchive.tar", diasEntries);

            ArchiveFormatValidator.ValidateAsFormat(diasTarArchive, format).Result
                .ValidationResult.Should().Be(ArchiveFormatValidationResult.Valid);

            Delete(diasTarArchive);
        }

        [Fact]
        public void DiasStructureValidator_ShouldInValidateN5SIPZipArchive()
        {
            const ArchiveFormat format = ArchiveFormat.DiasSipN5;

            DiasEntry[] diasEntries = DiasProvider.ProvideForFormat(format).GetEntries();

            DiasEntry[] excludedDiasEntries = { diasEntries[0], diasEntries[1] };
            DiasEntry[] invalidDiasEntrySet = diasEntries.Except(excludedDiasEntries).ToArray();
            IEnumerable<string> namesOfExcludedEntries = excludedDiasEntries.Select(e => e.Name);

            FileInfo diasTarArchive = CreateDiasTarArchive("testArchive.tar", invalidDiasEntrySet);

            ArchiveFormatValidationReport validationReport =
                ArchiveFormatValidator.ValidateAsFormat(diasTarArchive, format).Result;

            validationReport.ValidationResult.Should().Be(ArchiveFormatValidationResult.Invalid);
            validationReport.ValidationSummary().Should().ContainAll(namesOfExcludedEntries);

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

                    CreateDiasTarArchiveEntries(diasSubDirectory.GetEntries(), Path.Join(path, diasSubDirectory.Name), archive);
                }

                archive.PutNextEntry(tarEntry);
                archive.CloseEntry();
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
