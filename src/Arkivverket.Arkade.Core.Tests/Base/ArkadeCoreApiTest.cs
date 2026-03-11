using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Tar;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base;

public class ArkadeCoreApiTest(TestSessionLifeTimeFilesFixture fixture)
{
    [Fact]
    [Trait("Category", "Integration")]
    public void Noark3_DirectoryInput_ProducesValidPackage()
    {
        DirectoryInfo input = TestData.Directory("Archives", "Noark3", "extraction");

        RunScenario(input, ArchiveType.Noark3, expectedContentFiles:
        [
            "content/addml.xml",
            "content/ARKIV.DAT",
            "content/DOK.DAT",
            "content/SAK.DAT"
        ]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark3_DiasTarInput_ProducesValidPackage()
    {
        FileInfo input =
            TestData.File("Archives", "Noark3", "diasPackage", "8851c420-80e0-4681-b838-8eeb542d46f1.tar");

        RunScenario(input, ArchiveType.Noark3, expectedContentFiles:
        [
            "content/addml.xml",
            "content/ARKIV.DAT",
            "content/DOK.DAT",
            "content/SAK.DAT"
        ]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark4_DirectoryInput_ProducesValidPackage()
    {
        DirectoryInfo input = TestData.Directory("Archives", "Noark4", "extraction");

        RunScenario(input, ArchiveType.Noark4, expectedContentFiles:
        [
            "content/NOARKIH.XML",
            "content/DATA/ARKIV.XML"
        ]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark4_DiasTarInput_ProducesValidPackage()
    {
        FileInfo input =
            TestData.File("Archives", "Noark4", "diasPackage", "ffb1fda0-5b13-478f-9e4a-d68e8a944399.tar");

        RunScenario(input, ArchiveType.Noark4, expectedContentFiles:
        [
            "content/NOARKIH.XML",
            "content/DATA/ARKIV.XML"
        ]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark5_DirectoryInput_ProducesValidPackage()
    {
        DirectoryInfo input = TestData.Directory("Archives", "Noark5", "extraction");

        RunScenario(input, ArchiveType.Noark5, expectedContentFiles:
        [
            "content/addml.xsd",
            "content/arkivstruktur.xml",
            "content/arkivuttrekk.xml",
            "content/dokumenter/5000000.pdf",
            "content/dokumenter/5000001.pdf"
        ]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark5_DiasTarInput_ProducesValidPackage()
    {
        FileInfo input = TestData.File("Archives", "Noark5", "diasPackage", "4b73981c-1fab-4d4c-91e7-fcc6a3bc057f.tar");

        RunScenario(input, ArchiveType.Noark5, expectedContentFiles:
        [
            "content/addml.xsd",
            "content/arkivstruktur.xml",
            "content/arkivuttrekk.xml",
            "content/dokumenter/5000000.pdf",
            "content/dokumenter/5000001.pdf"
        ]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SpecializedSystem_DirectoryInput_ProducesValidPackage()
    {
        DirectoryInfo input = TestData.Directory("Archives", "SpecializedSystem", "extraction");

        RunScenario(input, ArchiveType.SpecializedSystem, expectedContentFiles:
        [
            "content/addml.xml",
            "content/ut_jeger.dat"
        ]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SpecializedSystem_DiasTarInput_ProducesValidPackage()
    {
        FileInfo input = TestData.File("Archives", "SpecializedSystem", "diasPackage",
            "bf193afe-4483-4481-b457-e9ba4f19681c.tar");

        RunScenario(input, ArchiveType.SpecializedSystem, expectedContentFiles:
        [
            "content/addml.xml",
            "content/ut_jeger.dat"
        ]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Siard_FileInput_ProducesValidPackage()
    {
        FileInfo input = TestData.File("Archives", "Siard", "extraction", "dbptk.siard");

        RunScenario(input, ArchiveType.Siard, expectedContentFiles:
        [
            "content/dbptk.siard"
        ]);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Siard_DiasTarInput_ProducesValidPackage()
    {
        FileInfo input = TestData.File("Archives", "Siard", "diasPackage", "841c0a18-7308-4407-b421-3efaa420d891.tar");

        RunScenario(input, ArchiveType.Siard, expectedContentFiles:
        [
            "content/dbptk.siard"
        ]);
    }

    private void RunScenario(FileSystemInfo input, ArchiveType archiveType, string[] expectedContentFiles,
        [CallerMemberName] string callerMemberName = null)
    {
        DirectoryInfo isolatedTemporaryDirectory = fixture.CreateIsolatedDirectory<ArkadeCoreApiTest>(callerMemberName);
        
        ArkadeProcessingArea.Establish(isolatedTemporaryDirectory.FullName);

        using var arkade = new Core.Base.Arkade();

        // 1. Load Archive
        Archive archive = arkade.LoadArchiveExtraction(input, archiveType);
        archive.ArchiveType.Should().Be(archiveType);

        // 2. Create Test Session and 3. Run Tests (if supported)
        if (archiveType != ArchiveType.Noark4)
        {
            TestSession testSession = arkade.CreateTestSession(archive);
            archive.TestSession = testSession;

            if (archive.IsTestable(out _))
            {
                arkade.RunTests(archive);
                archive.TestSession.TestSuite.Should().NotBeNull();
            }
        }

        // 4. Create Package
        // Need to set OutputDiasPackage before CreatePackage, normally done in GUI/CLI
        // Inspired by InformationPackageCreatorTest.cs:254
        var archiveMetadata = new ArchiveMetadata();
        archive.OutputDiasPackage = new OutputDiasPackage(
            PackageType.SubmissionInformationPackage, archiveMetadata, archive.ProcessingDirectory);

        string outputDirectory = isolatedTemporaryDirectory.CreateSubdirectory("output").FullName;

        arkade.CreatePackage(archive, SupportedLanguage.nb, generateFileFormatInfo: false, outputDirectory);

        // 5. Verify Package Content
        VerifyPackage(archive, outputDirectory, expectedContentFiles);
    }

    private static void VerifyPackage(Archive archive, string outputDirectory, string[] expectedContentFiles)
    {
        Uuid packageId = archive.OutputDiasPackage.Id;
        string resultsDirectoryName = string.Format(OutputFileNames.ResultOutputDirectory, packageId);
        string resultsDirectoryPath = Path.Combine(outputDirectory, resultsDirectoryName);

        // Verify files in the result directory
        List<string> resultFiles = Directory.GetFiles(resultsDirectoryPath).Select(Path.GetFileName).ToList();
        resultFiles.Should().Contain($"{packageId}.tar");
        resultFiles.Should().Contain($"{packageId}.xml");

        string tarFilePath = Path.Combine(resultsDirectoryPath, $"{packageId}.tar");
        List<string> packageFileList = GetFileListFromTarArchive(tarFilePath);

        string rootDirInTar = packageId + "/";
        foreach (string expectedFile in expectedContentFiles)
        {
            packageFileList.Should().Contain(rootDirInTar + expectedFile);
        }

        // Verify metadata (inspired by InformationPackageCreatorTest.GetFileListFromMetadata)
        List<string> metadataFileList = archive.OutputDiasPackage.ArchiveMetadata.FileDescriptions
            .Select(f => (packageId + "/" + f.Name).Replace('\\', '/')).ToList();

        List<string> packageFilesExpectedInMetadata = GetPackageItemsExpectedInMetadata(packageFileList);

        metadataFileList.Should().BeEquivalentTo(packageFilesExpectedInMetadata);
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

    private static List<string>
        GetPackageItemsExpectedInMetadata(
            List<string> packageFileList) // TODO: Reuse together with InformationPackageCreatorTest.GetPackageItemsExpectedInMetadata
    {
        return packageFileList.Where(item => IsNotADirectoryItem(item) && IsNotTheMetadataFile(item)).ToList();

        bool IsNotADirectoryItem(string fileCandidate) =>
            !Path.EndsInDirectorySeparator(fileCandidate) &&
            Path.HasExtension(fileCandidate) &&
            !packageFileList.Any(item => item.StartsWith(fileCandidate + '/') || item.StartsWith(fileCandidate + '\\'));

        bool IsNotTheMetadataFile(string fileListItem)
        {
            return !fileListItem.EndsWith(ArkadeConstants.DiasMetsXmlFileName);
        }
    }
}
