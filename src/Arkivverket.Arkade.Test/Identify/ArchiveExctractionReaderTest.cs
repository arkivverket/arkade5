using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Util;
using Moq;
using Xunit;

namespace Arkivverket.Arkade.Test.Identify
{
    public class ArchiveExctractionReaderTest
    {



        [Fact]
        public void DecompressAndIdentifyTarFiles()
        {
            var fileName = "c:\\does\\not\\exist.tar";
            var targetFolderName = "c:\\temp";
            var compressionUtilityMock = new Mock<ICompressionUtility>();
            //compressionUtilityMock.Setup(c => c.ExtractFolderFromArchive(fileName, targetFolderName)).Returns(0);

            new ArchiveExtractionReader(compressionUtilityMock.Object).ReadFromFile(fileName);

            compressionUtilityMock.Verify(c => c.ExtractFolderFromArchive(fileName, targetFolderName));

        }

    }
}
