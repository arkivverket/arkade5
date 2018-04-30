using System.Reflection;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Metadata;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Metadata
{
    public class DiasMetsCreatorTest : MetsCreatorTest
    {
        [Fact]
        public void ShouldSaveCreatedDiasMetsFileToDisk()
        {
            string pathToMetsFile = CreateMetsFile();

            File.Exists(pathToMetsFile).Should().BeTrue();
        }

        [Fact]
        public void DiasMetsFileForAipShouldReferenceEadXmlAndEacCpfXml()
        {
            string pathToMetsFileForAip = CreateMetsFile(PackageType.ArchivalInformationPackage);

            IsReferencingEadXmlAndEacCpfXml(pathToMetsFileForAip).Should().BeTrue();
        }


        [Fact]
        public void DiasMetsFileForSipShouldNotReferenceEadXmlOrEacCpfXml()
        {
            string pathToMetsFileForSip = CreateMetsFile(PackageType.SubmissionInformationPackage);

            IsReferencingEadXmlOrEacCpfXml(pathToMetsFileForSip).Should().BeFalse();
        }

        private string CreateMetsFile()
        {
            return CreateMetsFile(PackageType.SubmissionInformationPackage);
        }

        private string CreateMetsFile(PackageType packageType)
        {
            string workingDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\TestData\\Metadata\\DiasMetsCreator";

            Archive archive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .Build();

            new DiasMetsCreator().CreateAndSaveFile(archive, ArchiveMetadata, packageType);

            string metsFilePath = Path.Combine(workingDirectory, "dias-mets.xml");

            return metsFilePath;
        }

        private bool IsReferencingEadXmlAndEacCpfXml(string pathToMetsFile)
        {
            var metsXmlString = File.ReadAllText(pathToMetsFile);

            return metsXmlString.Contains(ArkadeConstants.EadXmlFileName) &&
                   metsXmlString.Contains(ArkadeConstants.EacCpfXmlFileName);
        }

        private static bool IsReferencingEadXmlOrEacCpfXml(string pathToMetsFile)
        {
            var metsXmlString = File.ReadAllText(pathToMetsFile);

            return metsXmlString.Contains(ArkadeConstants.EadXmlFileName) ||
                   metsXmlString.Contains(ArkadeConstants.EacCpfXmlFileName);
        }
    }
}
