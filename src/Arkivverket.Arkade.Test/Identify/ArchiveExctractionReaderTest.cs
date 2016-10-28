using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Util;
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
            var uuid = Uuid.Of("d1c9102e-7106-4355-a4a4-0c9b7f9b3541");
            string pathToExtractedFilesRegex = ArkadeConstants.GetArkadeWorkDirectory().FullName +
                                               Path.DirectorySeparatorChar + "..............-" + uuid +
                                               Path.DirectorySeparatorChar + uuid;
            pathToExtractedFilesRegex = pathToExtractedFilesRegex.Replace("\\", "\\\\");

            var extractorMock = new Mock<ICompressionUtility>();
            extractorMock.Setup(e => e.ExtractFolderFromArchive(It.IsAny<string>(), It.IsAny<string>()));

            var archiveType = ArchiveType.Noark5;
            var identifierMock = new Mock<IArchiveIdentifier>();
            identifierMock.Setup(i => i.Identify(It.IsAny<string>())).Returns(archiveType);

            var statusEventHandler = new StatusEventHandler();

            TestSession testSession =
                new TestSessionFactory(extractorMock.Object, identifierMock.Object, statusEventHandler)
                    .NewSessionFromTarFile(uuid + ".tar", null);

            var archive = testSession.Archive;
            archive.Should().NotBeNull();
            archive.Uuid.Should().Be(uuid);
            archive.WorkingDirectory.FullName.Should().MatchRegex(pathToExtractedFilesRegex);
            archive.ArchiveType.Should().Be(archiveType);

            Directory.Delete(archive.WorkingDirectory.Parent.FullName, true);
        }
    }
}