using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Tar;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base;

public class InformationPackageCreatorTest
{
    private readonly DirectoryArchiveContent _archiveContent =
        new(TestData.Directory(Path.Combine("Archives", "Noark5", "extraction")));

    private readonly ArchiveMetadata _archiveMetadata =
        MetadataExampleCreator.Create(MetadataExamplePurpose.InternalTesting);

    private readonly DirectoryInfo _tmpDirectory = TestData.Directory(".tmp");

    public InformationPackageCreatorTest()
    {
        if (_tmpDirectory.Exists)
            _tmpDirectory.Delete(true);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateSipTest()
    {
        (Uuid outputPackageId, List<string> metadataFileList, List<string> packageFileList) =
            CreatePackage(PackageType.SubmissionInformationPackage);

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

        // Files in total, including the directories implicit tested by subentries (above)
        packageFileList.Count.Should().Be(19);

        // All files in the package (except the metadata file itself) should be described in its metadata:
        List<string> packageFilesExpectedInMetadata = GetPackageItemsExpectedInMetadata(packageFileList);
        metadataFileList.Should().BeEquivalentTo(packageFilesExpectedInMetadata);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void CreateAipTest()
    {
        (Uuid outputPackageId, List<string> metadataFileList, List<string> packageFileList) =
            CreatePackage(PackageType.ArchivalInformationPackage);

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

        // Files in total, including the directories implicit tested by subentries (above)
        packageFileList.Count.Should().Be(22);

        // All files in the package (except the metadata file itself) should be described in its metadata:
        List<string> packageFilesExpectedInMetadata = GetPackageItemsExpectedInMetadata(packageFileList);
        metadataFileList.Should().BeEquivalentTo(packageFilesExpectedInMetadata);
    }

    private (Uuid, List<string>, List<string>) CreatePackage(PackageType packageType)
    {
        using var disposableDirectory = new DisposableDirectory(_tmpDirectory);
        DirectoryInfo processingDirectory = disposableDirectory.Get().CreateSubdirectory("processing");
        Archive archive = new ArchiveBuilder(_archiveContent, processingDirectory).Build<Noark5Archive>();

        archive.OutputDiasPackage = new OutputDiasPackage(packageType, _archiveMetadata, archive.ProcessingDirectory);

        string outputDirectory = disposableDirectory.Get().CreateSubdirectory("output").FullName;

        string packageFilePath = packageType switch // NB! UUID-origin
        {
            PackageType.SubmissionInformationPackage => InformationPackageCreator().CreateSip(archive, outputDirectory),
            PackageType.ArchivalInformationPackage => InformationPackageCreator().CreateAip(archive, outputDirectory),
            _ => null
        };

        return (
            archive.OutputDiasPackage.Id, // NB! UUID-writeout (unit testing)
            GetFileListFromMetadata(archive.OutputDiasPackage),
            GetFileListFromTarArchive(packageFilePath)
        );
    }

    private static InformationPackageCreator InformationPackageCreator()
    {
        var metadataFilesCreator = new MetadataFilesCreator(
            new DiasMetsCreator(), new DiasPremisCreator(), new EadCreator(), new EacCpfCreator(), new LogCreator()
        );
        var statusEventHandler = new StatusEventHandler();
        var siardMetadataFileHelper = new SiardMetadataFileHelper(new SiardArchiveReader());

        return new InformationPackageCreator(metadataFilesCreator, statusEventHandler, siardMetadataFileHelper);
    }

    private static List<string> GetFileListFromMetadata(OutputDiasPackage outputDiasPackage)
    {
        List<FileDescription> fileDescriptions = outputDiasPackage.ArchiveMetadata.FileDescriptions;

        return fileDescriptions.Select(f => (outputDiasPackage.Id + "/" + f.Name).Replace('\\', '/')).ToList();
    }

    private static List<string> GetFileListFromTarArchive(string tarArchiveFilePath)
    {
        var fileList = new List<string>();

        using Stream inStream = File.OpenRead(tarArchiveFilePath);
        using var tarArchive = TarArchive.CreateInputTarArchive(inStream, Encoding.Latin1);
        tarArchive.ProgressMessageEvent += (_, entry, _) => fileList.Add(entry.Name);
        tarArchive.ListContents();

        return fileList;
    }

    private static List<string> GetPackageItemsExpectedInMetadata(List<string> packageFileList)
    {
        return packageFileList.Where(item => IsNotADirectoryItem(item) && IsNotTheMetadataFile(item)).ToList();

        bool IsNotADirectoryItem(string fileListItem)
        {
            return Path.HasExtension(fileListItem);
        }

        bool IsNotTheMetadataFile(string fileListItem)
        {
            return !fileListItem.EndsWith("dias-mets.xml");
        }
    }
}
