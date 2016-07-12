using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using FluentAssertions;
using Moq;
using Xunit;

namespace Arkivverket.Arkade.Test.Identify
{
    public class ArchiveExctractionReaderTest
    {
        [Fact]
        public void ExtractAndIdentifyTarFiles()
        {
            var uuid = "d1c9102e-7106-4355-a4a4-0c9b7f9b3541";
            var fileName = $"c:\\users\\johnsmith\\my documents\\{uuid}.tar";
            string pathToExtractedFiles = $"c:\\temp\\{uuid}";

            var extractorMock = new Mock<IArchiveExtractor>();
            extractorMock.Setup(e => e.Extract(fileName)).Returns(new ArchiveExtraction(uuid, pathToExtractedFiles));

            var archiveExtraction = new ArchiveExtractionReader(extractorMock.Object).ReadFromFile(fileName);
            archiveExtraction.Uuid.Should().Be(uuid);
            archiveExtraction.WorkingDirectory.Should().Be(pathToExtractedFiles);
        }
    }
}