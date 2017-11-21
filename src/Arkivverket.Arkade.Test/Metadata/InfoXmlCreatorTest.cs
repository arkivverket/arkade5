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

            var packageFileName = Path.Combine(workingDirectory, "package.tar");

            new InfoXmlCreator().CreateAndSaveFile(ArchiveMetadata, packageFileName);

            string infoXmlFilePath = Path.Combine(workingDirectory, "info.xml");

            File.Exists(infoXmlFilePath).Should().BeTrue();

            File.Delete(infoXmlFilePath);
        }
    }
}
