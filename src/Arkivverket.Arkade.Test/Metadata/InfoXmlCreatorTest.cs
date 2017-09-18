using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Metadata;
using Arkivverket.Arkade.Test.Core;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Metadata
{
    public class InfoXmlCreatorTest : MetsCreatorTest
    {
        [Fact]
        public void ShouldSaveCreatedInfoXmlFileToDisk()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Metadata\\InfoXmlCreator";

            Archive archive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .Build();

            var packageFileName = Path.Combine(workingDirectory, archive.Uuid + ".tar");

            new InfoXmlCreator().CreateAndSaveFile(archive, ArchiveMetadata, packageFileName);

            string infoXmlFilePath = Path.Combine(workingDirectory, archive.Uuid + ".xml");

            File.Exists(infoXmlFilePath).Should().BeTrue();
        }
    }
}
