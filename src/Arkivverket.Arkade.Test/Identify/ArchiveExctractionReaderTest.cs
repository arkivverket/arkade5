using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Test.Core;
using FluentAssertions;
using Moq;
using System.IO;
using Xunit;

namespace Arkivverket.Arkade.Test.Identify
{
    public class ArchiveExctractionReaderTest
    {
        [Fact]
        public void ExtractAndIdentifyTarFiles()
        {
            var uuid = Uuid.Of("d1c9102e-7106-4355-a4a4-0c9b7f9b3541");
            string pathToExtractedFiles = $"c:\\temp\\{uuid}";

            var extractorMock = new Mock<IArchiveExtractor>();
            extractorMock.Setup(e => e.Extract(It.IsAny<FileInfo>())).Returns(new DirectoryInfo(pathToExtractedFiles));

            var archiveType = ArchiveType.Noark5;
            var identifierMock = new Mock<IArchiveIdentifier>();
            identifierMock.Setup(i => i.Identify(It.IsAny<string>())).Returns(archiveType);

            TestSession testSession = new Arkade.Identify.TestSessionBuilder(extractorMock.Object, identifierMock.Object).NewSessionFromTarFile(uuid+".tar", "info.xml");

            var archive = testSession.Archive;
            archive.Should().NotBeNull();
            archive.Uuid.Should().Be(uuid);
            archive.WorkingDirectory.FullName.Should().Be(pathToExtractedFiles);
            archive.ArchiveType.Should().Be(archiveType);
        }
    }
}