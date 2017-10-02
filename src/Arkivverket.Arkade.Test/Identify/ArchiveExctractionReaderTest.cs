using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Moq;
using Xunit;
using Arkivverket.Arkade.Test.Tests.Noark5;

namespace Arkivverket.Arkade.Test.Identify
{
    public class ArchiveExctractionReaderTest
    {
        [Fact(Skip = "What is the purpose of this test?")]
        public void ExtractAndIdentifyTarFiles()
        {
            var uuid = Uuid.Of("c3db9d4e-720c-4f75-bfb6-de90231dc44c");
            string pathToExtractedFilesRegex = ArkadeProcessingArea.GetWorkDirectory().FullName +
                                               Path.DirectorySeparatorChar + "..............-" + uuid +
                                               Path.DirectorySeparatorChar + uuid;
            pathToExtractedFilesRegex = pathToExtractedFilesRegex.Replace("\\", "\\\\");

            var extractorMock = new Mock<ICompressionUtility>();
            extractorMock.Setup(e => e.ExtractFolderFromArchive(It.IsAny<FileInfo>(), It.IsAny<DirectoryInfo>()));

            var archiveType = ArchiveType.Noark5;
            var identifierMock = new Mock<IArchiveIdentifier>();
            identifierMock.Setup(i => i.Identify(It.IsAny<string>())).Returns(archiveType);

            var statusEventHandler = new StatusEventHandler();

            string file = TestUtil.TestDataDirectory + Path.DirectorySeparatorChar + "tar" + Path.DirectorySeparatorChar + "Noark3-eksempel-1" + Path.DirectorySeparatorChar + uuid + ".tar";
            TestSession testSession =
                new TestSessionFactory(extractorMock.Object, statusEventHandler)
                    .NewSession(ArchiveFile.Read(file,archiveType));

            var archive = testSession.Archive;
            archive.Should().NotBeNull();
            archive.Uuid.Should().Be(uuid);
            archive.WorkingDirectory.Root().DirectoryInfo().FullName.Should().MatchRegex(pathToExtractedFilesRegex);
            archive.ArchiveType.Should().Be(archiveType);

            System.IO.Directory.Delete(archive.WorkingDirectory.Root().DirectoryInfo().FullName, true);
        }
    }
}