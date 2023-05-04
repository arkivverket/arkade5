using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Moq;
using Xunit;
using Arkivverket.Arkade.Core.Tests.Testing.Noark5;

namespace Arkivverket.Arkade.Core.Tests.Identify
{
    public class ArchiveExctractionReaderTest
    {
        [Fact(Skip = "What is the purpose of this test?")]
        public void ExtractAndIdentifyTarFiles()
        {
            var uuid = Uuid.Of("c3db9d4e-720c-4f75-bfb6-de90231dc44c"); // NB! UUID-origin
            string pathToExtractedFilesRegex = Path.Combine(
                ArkadeProcessingArea.WorkDirectory.FullName, "..............-" + uuid, uuid.ToString() // NB! UUID-writeout
            );
            pathToExtractedFilesRegex = pathToExtractedFilesRegex.Replace("\\", "\\\\");

            var extractorMock = new Mock<ICompressionUtility>();
            extractorMock.Setup(e => e.ExtractFolderFromArchive(It.IsAny<FileInfo>(), It.IsAny<DirectoryInfo>(), false, uuid.ToString()));

            var archiveType = ArchiveType.Noark5;
            var identifierMock = new Mock<IArchiveTypeIdentifier>();
            identifierMock.Setup(i => i.IdentifyTypeOfChosenArchiveFile(It.IsAny<string>())).Returns(archiveType);

            var statusEventHandler = new StatusEventHandler();

            string file = Path.Combine(TestUtil.TestDataDirectory, "tar", "Noark3-eksempel-1", uuid + ".tar"); // NB! UUID-writeout
            TestSession testSession =
                new TestSessionFactory(extractorMock.Object, statusEventHandler)
                    .NewSession(ArchiveFile.Read(file,archiveType));

            var archive = testSession.Archive;
            archive.Should().NotBeNull();
            archive.Uuid.Should().Be(uuid);
            archive.WorkingDirectory.Root().DirectoryInfo().FullName.Should().MatchRegex(pathToExtractedFilesRegex);
            archive.ArchiveType.Should().Be(archiveType);

            Directory.Delete(archive.WorkingDirectory.Root().DirectoryInfo().FullName, true);
        }
    }
}