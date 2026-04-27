using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base;

public class ArkadeCoreApiTest(TestSessionLifeTimeFilesFixture fixture)
{
    private const SupportedLanguage OutputLanguage = SupportedLanguage.en;
    
    [Fact]
    [Trait("Category", "Integration")]
    public void Noark3_DirectoryInput_ProducesValidPackage()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "Noark3", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.Noark3, expectedContentPaths);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark3_DiasTarInput_ProducesValidPackage()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Noark3", "diasPackage", "8851c420-80e0-4681-b838-8eeb542d46f1.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Noark3, expectedContentFiles);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark4_DirectoryInput_ProducesValidPackage()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "Noark4", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.Noark4, expectedContentPaths);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark4_DiasTarInput_ProducesValidPackage()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Noark4", "diasPackage", "ffb1fda0-5b13-478f-9e4a-d68e8a944399.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Noark4, expectedContentFiles);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark5_DirectoryInput_ProducesValidPackage()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "Noark5", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.Noark5, expectedContentPaths);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark5_DiasTarInput_ProducesValidPackage()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Noark5", "diasPackage", "4b73981c-1fab-4d4c-91e7-fcc6a3bc057f.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Noark5, expectedContentFiles);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SpecializedSystem_DirectoryInput_ProducesValidPackage()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "SpecializedSystem", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.SpecializedSystem, expectedContentPaths);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SpecializedSystem_DiasTarInput_ProducesValidPackage()
    {
        FileInfo diasTarFile = TestData.File("Archives", "SpecializedSystem", "diasPackage",
            "bf193afe-4483-4481-b457-e9ba4f19681c.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.SpecializedSystem, expectedContentFiles);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Siard_FileInput_ProducesValidPackage()
    {
        FileInfo siardFile = TestData.File("Archives", "Siard", "extraction", "dbptk.siard");

        DirectoryInfo directoryWithExpectedContentFiles = TestData.Directory("Archives", "Siard", "extraction");

        const string unReferencedFile =
            "t01bclob12_dbptk-desktop-2.5.9_ext.siard_lobseg_1/content/schema1/table1/lob9/unreferenced-file.bin";

        string[] expectedContentFilePaths = GetPathsAsWhenInTar(directoryWithExpectedContentFiles, filePathsOnly: true)
            .Except([unReferencedFile]).ToArray();

        RunScenario(siardFile, ArchiveType.Siard, expectedContentFilePaths);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Siard_DiasTarInput_ProducesValidPackage()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Siard", "diasPackage", "841c0a18-7308-4407-b421-3efaa420d891.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Siard, expectedContentFiles);
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
            archive.TestSession.OutputLanguage = OutputLanguage;
            
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

        arkade.CreatePackage(archive, OutputLanguage, generateFileFormatInfo: false, outputDirectory);

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
        var tarFileRootDirectory = $"{packageId}/";

        List<string> packageFileList =
            DiasTarArchiveUtility.GetFileList(tarFilePath)
                .Select(Path.TrimEndingDirectorySeparator).ToList();

        List<string> expectedPackageFileList =
            CreateExpectedPackageFileList(archive.ArchiveType, tarFileRootDirectory, expectedContentFiles)
                .Select(Path.TrimEndingDirectorySeparator).ToList();

        // Use to examine the actual difference between produced and expected package file contents:
        //IEnumerable<string> filesInPackageNotExpected = packageFileList.ExceptOnce(expectedPackageFileList);
        //IEnumerable<string> expectedFilesNotInPackage = expectedPackageFileList.ExceptOnce(packageFileList);
        //filesInPackageNotExpected.Should().BeEmpty();
        //expectedFilesNotInPackage.Should().BeEmpty();

        packageFileList.Should().BeEquivalentTo(expectedPackageFileList);

        // Verify metadata (inspired by InformationPackageCreatorTest.GetFileListFromMetadata)
        List<string> metadataFileList = archive.OutputDiasPackage.ArchiveMetadata.FileDescriptions
            .Select(f => (packageId + "/" + f.Name).Replace('\\', '/')).ToList();

        List<string> packageFilesExpectedInMetadata =
            DiasTarArchiveUtility.GetPackageItemsExpectedInMetadata(packageFileList);

        metadataFileList.Should().BeEquivalentTo(packageFilesExpectedInMetadata);
    }

    private static string[] CreateExpectedPackageFileList(ArchiveType archiveType, string rootDirectory, string[] expectedContentFiles)
    {
        return
        [
            rootDirectory,
            .. DiasTarArchiveUtility.GetArkadeAppliedAipFilesList(archiveType).Select(file => rootDirectory + file),
            .. expectedContentFiles.Select(file => $"{rootDirectory}{ArkadeConstants.DirectoryNameContent}/{file}")
        ];
    }

    private static string[] GetPathsAsWhenInTar(DirectoryInfo directory, bool filePathsOnly = false)
    {
        IEnumerable<FileSystemInfo> items = filePathsOnly
            ? directory.EnumerateFiles("*", SearchOption.AllDirectories)
            : directory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories);

        return items.Select(item =>
            item.FullName[(directory.FullName.TrimEnd('/', '\\').Length + 1)..].Replace('\\', '/')).ToArray();
    }
}
