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

            archiveFileFormats.FileFormats.FirstOrDefault(f => f.Puid.Equals("fmt/817"))?.ValidFrom.Should().Be(new DateOnly(2022, 3, 1));
        }
    }
}
