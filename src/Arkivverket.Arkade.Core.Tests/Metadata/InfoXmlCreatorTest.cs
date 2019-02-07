using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Tests.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Metadata
{
    public class InfoXmlCreatorTest : MetsCreatorTest
    {
        [Fact]
        public void ShouldSaveCreatedInfoXmlFileToDisk()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Metadata\\InfoXmlCreator";

            var packageFileName = Path.Combine(workingDirectory, "package.tar");
            var infoXmlFileName = Path.Combine(workingDirectory, "UUID.xml");

            new InfoXmlCreator().CreateAndSaveFile(ArchiveMetadata, packageFileName, infoXmlFileName);

            string infoXmlFilePath = Path.Combine(workingDirectory, infoXmlFileName);

            File.Exists(infoXmlFilePath).Should().BeTrue();

            File.Delete(infoXmlFilePath);
        }
    }
}
