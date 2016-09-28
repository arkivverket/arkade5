using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using FluentAssertions;
using Moq;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
{
    public class TestSessionBuilderTest
    {
        [Fact]
        public void ShouldBuildNewTestSessionFromTarFile()
        {

            var metadataFileName = "c:\\temp\\info.xml";
            var archiveFileName = "c:\\temp\\myfile.tar";
            var uuid = "uuid";
            var workingDirectory = "c:\\temp";
            var archiveType = ArchiveType.Noark5;

            var archiveExtractorMock = new Mock<IArchiveExtractor>();
            archiveExtractorMock.Setup(a => a.Extract(archiveFileName)).Returns(new Archive(uuid, workingDirectory));

            var archiveIdentifierMock = new Mock<IArchiveIdentifier>();
            archiveIdentifierMock.Setup(i => i.Identify(metadataFileName)).Returns(archiveType);

            var builder = new TestSessionBuilder(archiveExtractorMock.Object, archiveIdentifierMock.Object);
            var testSession = builder.NewSessionFromTarFile(archiveFileName, metadataFileName);

            testSession.Should().NotBeNull();
            testSession.Archive.Should().NotBeNull();
            testSession.Archive.Uuid.Should().Be(uuid);
            testSession.Archive.WorkingDirectory.Should().Be(workingDirectory);
            testSession.Archive.ArchiveType.Should().Be(archiveType);
        }
    }
}