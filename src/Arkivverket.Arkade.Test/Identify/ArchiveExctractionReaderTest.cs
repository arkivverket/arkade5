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
            string pathToExtractedFiles = $"c:\\temp\\{uuid}";
            var extractorMock = new Mock<IArchiveExtractor>();
            extractorMock.Setup(e => e.Extract(It.IsAny<string>())).Returns(new ArchiveExtraction(uuid, pathToExtractedFiles));

            var archiveType = ArchiveType.Noark5;
            var identifierMock = new Mock<IArchiveIdentifier>();
            identifierMock.Setup(i => i.Identify(It.IsAny<string>())).Returns(archiveType);

            var archiveExtraction = new ArchiveExtractionReader(extractorMock.Object, identifierMock.Object).ReadFromFile("archive.tar", "info.xml");

            archiveExtraction.Uuid.Should().Be(uuid);
            archiveExtraction.WorkingDirectory.Should().Be(pathToExtractedFiles);
            archiveExtraction.ArchiveType.Should().Be(archiveType);
        }
    }
}