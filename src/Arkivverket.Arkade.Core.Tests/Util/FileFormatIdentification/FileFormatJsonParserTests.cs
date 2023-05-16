using System;
using System.Linq;
using Xunit;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.Tests.Util.FileFormatIdentification
{
    public class FileFormatJsonParserTests
    {
        [Fact]
        public void ShouldParseFileFormatsJson()
        {
            ArchiveFileFormats archiveFileFormats = FileFormatsJsonParser.ParseArchiveFileFormats();

            archiveFileFormats.FileFormats.Should().Contain(f => f.Puid.Contains("fmt/817"));

            ArchiveFileFormat jsonFileFormat = archiveFileFormats.FileFormats.First(f => f.Puid.Contains("fmt/817"));

            jsonFileFormat.ValidFrom.Should().Be(new DateTime(2022, 3, 1));
            jsonFileFormat.ValidTo.Should().BeNull();
            jsonFileFormat.AdditionalRequirements.Should().NotBeEmpty();
        }
    }
}
