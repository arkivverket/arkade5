using System;
using System.Collections.Generic;
using Xunit;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.Tests.Util.FileFormatIdentification
{
    public class ArchiveFileFormatValidatorTests : LanguageDependentTest
    {
        [Fact]
        public void ShouldValidateArchiveFileFormats()
        {
            var archiveFileFormats = new List<ArchiveFileFormat>
            {
                new() { Puid = new[] { "fmt/1" }, ValidFrom = new DateOnly(2022, 3, 1), AdditionalRequirements = "" },
                new() { Puid = new[] { "fmt/2" }, ValidTo = new DateOnly(2007, 3, 1), AdditionalRequirements = "" },
                new() { Puid = new[] { "fmt/3" }, ValidTo = DateOnly.MaxValue, AdditionalRequirements = "" },
                new() { Puid = new[] { "fmt/4" }, ValidFrom = new DateOnly(2024, 3, 1), AdditionalRequirements = "" },
                new() { Puid = new[] { "fmt/5" }, AdditionalRequirements = "yes" },
            };

            ArchiveFileFormatValidator.Initialize(archiveFileFormats);

            ArchiveFileFormatValidator.Validate("fmt/1").Should().Be("Gyldig");
            ArchiveFileFormatValidator.Validate("fmt/2").Should().Be("Ikke gyldig");
            ArchiveFileFormatValidator.Validate("fmt/3").Should().Be("Gyldig");
            ArchiveFileFormatValidator.Validate("fmt/4").Should().Be("Ikke gyldig");
            ArchiveFileFormatValidator.Validate("fmt/5").Should().Be("Gyldig*");
        }
    }
}
