using Xunit;
using Arkivverket.Arkade.Core.Base.Siard;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.Tests.Base.Siard
{
    public class SiardXmlTableReaderTests
    {
        [Fact]
        public void GetFormatAnalysedLobsFromSiard2_1FileTest()
        {
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string siardArchivePath = Path.Combine("TestData", "Siard", "testuttrekk_med_blobs.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(siardArchivePath);

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(9);
            foreach (var lob in formatAnalysedLobs)
            {
                _ = lob.FileExtension.Should().Be(".bin");
            }
        }

        [Fact]
        public void GetFormatAnalysedLobsFromSiard1_0FileTest()
        {
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string archiveFolder = Path.Combine("TestData", "Siard", "siard1_med_blobs.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(archiveFolder);

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(10);
        }
    }
}