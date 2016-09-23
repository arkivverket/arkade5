using System.IO;
using Arkivverket.Arkade.Core;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
{
    public class ArchiveExtractionTest
    {

        [Fact]
        public void ShouldReturnContentDescriptionFileNameForNoark5()
        {
            var workingDirectory = "c:\\temp";
            var archiveExtraction = new Archive("uuid", workingDirectory);

            archiveExtraction.GetContentDescriptionFileName().Should().Be($"{workingDirectory}{Path.DirectorySeparatorChar}arkivstruktur.xml");
        }

        [Fact] 
        public void ShouldReturnStructureDescriptionFileNameForNoark5()
        {
            var workingDirectory = "c:\\temp";
            var archiveExtraction = new Archive("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;
            archiveExtraction.GetStructureDescriptionFileName().Should().Be($"{workingDirectory}{Path.DirectorySeparatorChar}arkivuttrekk.xml");
        }

        [Fact]
        public void ShouldReturnStructureDescriptionFileNameForNoark4()
        {
            var workingDirectory = "c:\\temp";
            var archiveExtraction = new Archive("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark4;
            archiveExtraction.GetStructureDescriptionFileName().Should().Be($"{workingDirectory}{Path.DirectorySeparatorChar}addml.xml");
        }
    }
}
