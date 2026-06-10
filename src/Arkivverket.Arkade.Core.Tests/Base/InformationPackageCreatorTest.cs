using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using System.Formats.Tar;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base;

public class InformationPackageCreatorTest(TestSessionLifeTimeFilesFixture testSessionLifeTimeFilesFixture)
{
    private readonly ArchiveMetadata _archiveMetadata =
        MetadataExampleCreator.Create(MetadataExamplePurpose.InternalTesting);

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateSipTest()
    {
        var content = new DirectoryArchiveContent(TestData.Directory("Archives", "Noark5", "extraction"));

        (Uuid outputPackageId, List<string> metadataFileList, List<string> packageFileList) =
            CreatePackage<Noark5Archive>(content, PackageType.SubmissionInformationPackage);

        string rootDir = outputPackageId + "/";

        // Arkade-generated files/directories:
        packageFileList.Should().Contain(rootDir);
        packageFileList.Should().Contain(rootDir + "dias-mets.xml");
        packageFileList.Should().Contain(rootDir + "dias-mets.xsd");
        packageFileList.Should().Contain(rootDir + "log.xml");
        packageFileList.Should().Contain(rootDir + "descriptive_metadata/"); // TODO: Should this empty directory be included?
        packageFileList.Should().Contain(rootDir + "administrative_metadata/");
        packageFileList.Should().Contain(rootDir + "administrative_metadata/addml.xsd");
        packageFileList.Should().Contain(rootDir + "administrative_metadata/arkivuttrekk.xml");
        packageFileList.Should().Contain(rootDir + "administrative_metadata/dias-premis.xml");
        packageFileList.Should().Contain(rootDir + "administrative_metadata/dias-premis.xsd");

        // Files from the input archive (content):
        packageFileList.Should().Contain(rootDir + "content/addml.xsd");
        packageFileList.Should().Contain(rootDir + "content/arkivstruktur.xml");
        packageFileList.Should().Contain(rootDir + "content/arkivstruktur.xsd");
        packageFileList.Should().Contain(rootDir + "content/arkivuttrekk.xml");
        packageFileList.Should().Contain(rootDir + "content/dokumenter/5000000.pdf");
        packageFileList.Should().Contain(rootDir + "content/dokumenter/5000001.pdf");
        packageFileList.Should().Contain(rootDir + "content/metadatakatalog.xsd");

        // Files that are not part of an SIP:
        packageFileList.Should().NotContain(rootDir + "administrative_metadata/repository_operations/");
        packageFileList.Should().NotContain(rootDir + "descriptive_metadata/eac-cpf.xml");
        packageFileList.Should().NotContain(rootDir + "descriptive_metadata/ead.xml");

        // Files in total, including the directories implicitly tested by subentries (above)
        packageFileList.Count.Should().Be(19);

        // All files in the package (except the metadata file itself) should be described in its metadata:
        List<string> packageFilesExpectedInMetadata = DiasTarArchiveUtility.GetPackageItemsExpectedInMetadata(packageFileList);
        metadataFileList.Should().BeEquivalentTo(packageFilesExpectedInMetadata);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateAipTest()
    {
        var content = new DirectoryArchiveContent(TestData.Directory("Archives", "Noark5", "extraction"));

        (Uuid outputPackageId, List<string> metadataFileList, List<string> packageFileList) =
            CreatePackage<Noark5Archive>(content, PackageType.ArchivalInformationPackage);

        string rootDir = outputPackageId + "/";

        // Arkade-generated files/directories:
        packageFileList.Should().Contain(rootDir);
        packageFileList.Should().Contain(rootDir + "dias-mets.xml");
        packageFileList.Should().Contain(rootDir + "dias-mets.xsd");
        packageFileList.Should().Contain(rootDir + "log.xml");
        packageFileList.Should().Contain(rootDir + "descriptive_metadata/"); // TODO: Should this empty directory be included?
        packageFileList.Should().Contain(rootDir + "administrative_metadata/");
        packageFileList.Should().Contain(rootDir + "administrative_metadata/addml.xsd");
        packageFileList.Should().Contain(rootDir + "administrative_metadata/arkivuttrekk.xml");
        packageFileList.Should().Contain(rootDir + "administrative_metadata/dias-premis.xml");
        packageFileList.Should().Contain(rootDir + "administrative_metadata/dias-premis.xsd");

        // Files from the input archive (content):
        packageFileList.Should().Contain(rootDir + "content/addml.xsd");
        packageFileList.Should().Contain(rootDir + "content/arkivstruktur.xml");
        packageFileList.Should().Contain(rootDir + "content/arkivstruktur.xsd");
        packageFileList.Should().Contain(rootDir + "content/arkivuttrekk.xml");
        packageFileList.Should().Contain(rootDir + "content/dokumenter/5000000.pdf");
        packageFileList.Should().Contain(rootDir + "content/dokumenter/5000001.pdf");
        packageFileList.Should().Contain(rootDir + "content/metadatakatalog.xsd");

        // Files that only are part of an AIP:
        packageFileList.Should().Contain(rootDir + "administrative_metadata/repository_operations/");
        packageFileList.Should().Contain(rootDir + "descriptive_metadata/eac-cpf.xml");
        packageFileList.Should().Contain(rootDir + "descriptive_metadata/ead.xml");

        // Files in total, including the directories implicitly tested by subentries (above)
        packageFileList.Count.Should().Be(22);

        // All files in the package (except the metadata file itself) should be described in its metadata:
        List<string> packageFilesExpectedInMetadata = DiasTarArchiveUtility.GetPackageItemsExpectedInMetadata(packageFileList);
        metadataFileList.Should().BeEquivalentTo(packageFilesExpectedInMetadata);
    }

    private (Uuid, List<string>, List<string>) CreatePackage<TArchive>(IArchiveContent content, PackageType packageType,
        [CallerMemberName] string callerMemberName = null) where TArchive : Archive
   {
       DirectoryInfo isolatedDirectory =
           testSessionLifeTimeFilesFixture.CreateIsolatedDirectory<InformationPackageCreatorTest>(callerMemberName);
        
        DirectoryInfo processingDirectory = isolatedDirectory.CreateSubdirectory("processing");
        Archive archive = new ArchiveBuilder(content, processingDirectory).Build<TArchive>();

        archive.OutputDiasPackage = new OutputDiasPackage(packageType, _archiveMetadata, archive.ProcessingDirectory);

        string outputDirectory = isolatedDirectory.CreateSubdirectory("output").FullName;

        string packageFilePath = packageType switch // NB! UUID-origin
        {
            PackageType.SubmissionInformationPackage => InformationPackageCreator().CreateSip(archive, outputDirectory),
            PackageType.ArchivalInformationPackage => InformationPackageCreator().CreateAip(archive, outputDirectory),
            _ => null
        };
        
        List<string> resultFiles = GetFileListFromResultsDirectory(outputDirectory, archive.OutputDiasPackage.Id);
        resultFiles.Should().Contain($"{archive.OutputDiasPackage.Id}.tar"); // package file
        resultFiles.Should().Contain($"{archive.OutputDiasPackage.Id}.xml"); // metadata file
        resultFiles.Should().HaveCount(2);

        return (
            archive.OutputDiasPackage.Id, // NB! UUID-transfer (unit testing)
            GetFileListFromMetadata(archive.OutputDiasPackage),
            DiasTarArchiveUtility.GetFileList(packageFilePath)
        );
    }

    private static InformationPackageCreator InformationPackageCreator()
    {
        var metadataFilesCreator = new MetadataFilesCreator(
            new DiasMetsCreator(), new DiasPremisCreator(), new EadCreator(), new EacCpfCreator(), new LogCreator()
        );
        var statusEventHandler = new StatusEventHandler();
        var siardMetadataFileHelper = new SiardMetadataFileHelper(new SiardArchiveReader());

        return new InformationPackageCreator(metadataFilesCreator, statusEventHandler, siardMetadataFileHelper, new TestSessionXmlGenerator());
    }
    
    private static List<string> GetFileListFromResultsDirectory(string outputDirectory, Uuid packageId)
    {
        string resultsDirectoryName = string.Format(OutputFileNames.ResultOutputDirectory, packageId);
        string resultsDirectoryPath = Path.Combine(outputDirectory, resultsDirectoryName);
        
        return Directory.GetFiles(resultsDirectoryPath).Select(Path.GetFileName).ToList();
    }

    private static List<string> GetFileListFromMetadata(OutputDiasPackage outputDiasPackage)
    {
        List<FileDescription> fileDescriptions = outputDiasPackage.ArchiveMetadata.FileDescriptions;

        return fileDescriptions.Select(f => (outputDiasPackage.Id + "/" + f.Name).Replace('\\', '/')).ToList();
    }
}
