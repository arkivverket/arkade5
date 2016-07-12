using System.IO;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Moq;
using Xunit;

namespace Arkivverket.Arkade.Test.Identify
{
    public class ArchiveExtractorTest
    {
        [Fact]
        public void ShouldExtractPackageToTemporaryFolder()
        {
            var compressionUtilityMock = new Mock<ICompressionUtility>();

            var uuid = "d1c9102e-7106-4355-a4a4-0c9b7f9b3541";
            var inputFilename = "c:\\users\\johnsmith\\my documents\\" + uuid + ".tar";
            var archiveExtraction = new ArchiveExtractor(compressionUtilityMock.Object).Extract(inputFilename);

            archiveExtraction.WorkingDirectory.Should().Be(ArchiveExtractor.TemporaryFolder + Path.DirectorySeparatorChar + uuid);
            archiveExtraction.Uuid.Should().Be(uuid);
        }
    }
}