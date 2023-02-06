using System;
using System.Collections.Generic;
using Xunit;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.Tests.Util.FileFormatIdentification
{
    public class ArchiveFileFormatValidatorTests
    {
        [Fact]
        public void ShouldValidateArchiveFileFormats()
        {
            var archiveFileFormats = new List<ArchiveFileFormat>
            {
                new() { Puid = new[] { "fmt/1" }, ValidFrom = new DateOnly(2022, 3, 1) },
                new() { Puid = new[] { "fmt/2" }, ValidTo = new DateOnly(2007, 3, 1) },
                new() { Puid = new[] { "fmt/3" }, ValidTo = new DateOnly(2024, 3, 1) },
                new() { Puid = new[] { "fmt/4" }, ValidFrom = new DateOnly(2024, 3, 1) },
                new() { Puid = new[] { "fmt/5" } },
            };

            HashSet<string> validFormats = ArchiveFileFormatValidator.GetValidPuids(archiveFileFormats);

            validFormats.Should().Contain("fmt/1");
            validFormats.Should().NotContain("fmt/2");
            validFormats.Should().Contain("fmt/3");
            validFormats.Should().NotContain("fmt/4");
            validFormats.Should().Contain("fmt/5");
        }
    }
}
