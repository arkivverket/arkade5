using System.IO;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Moq;
using Xunit;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Identify
{
    public class ArchiveExtractorTest
    {
        [Fact]
        public void ShouldExtractPackageToTemporaryFolder()
        {
            var compressionUtilityMock = new Mock<ICompressionUtility>();

            var uuid = Uuid.Random();
            var inputFilename = new FileInfo("c:\\users\\johnsmith\\my documents\\" + uuid + ".tar");
            var archiveExtraction = new ArchiveExtractor(compressionUtilityMock.Object).Extract(inputFilename);

            archiveExtraction.FullName.Should().Be(ArchiveExtractor.TemporaryFolder + Path.DirectorySeparatorChar + uuid);
        }
    }
}