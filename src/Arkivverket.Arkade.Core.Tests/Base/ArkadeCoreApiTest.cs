using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using FluentAssertions;
using Xunit;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Tests.Base;

public class ArkadeCoreApiTest(TestSessionLifeTimeFilesFixture fixture)
{
    private const SupportedLanguage OutputLanguage = SupportedLanguage.en;
    private static PackageType SIP => PackageType.SubmissionInformationPackage;
    private static PackageType AIP => PackageType.ArchivalInformationPackage;
    
    [Fact]
    [Trait("Category", "Integration")]
    public void Noark3_DirectoryInput_ProducesValidSIP()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "Noark3", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.Noark3, expectedContentPaths, packageType: SIP);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void Noark3_DirectoryInput_ProducesValidAIP()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "Noark3", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.Noark3, expectedContentPaths, packageType: AIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark3_DiasTarInput_ProducesValidSIP()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Noark3", "diasPackage", "8851c420-80e0-4681-b838-8eeb542d46f1.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Noark3, expectedContentFiles, packageType: SIP);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void Noark3_DiasTarInput_ProducesValidAIP()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Noark3", "diasPackage", "8851c420-80e0-4681-b838-8eeb542d46f1.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Noark3, expectedContentFiles, packageType: AIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark4_DirectoryInput_ProducesValidSIP()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "Noark4", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.Noark4, expectedContentPaths, packageType: SIP);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void Noark4_DirectoryInput_ProducesValidAIP()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "Noark4", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.Noark4, expectedContentPaths, packageType: AIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark4_DiasTarInput_ProducesValidSIP()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Noark4", "diasPackage", "ffb1fda0-5b13-478f-9e4a-d68e8a944399.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Noark4, expectedContentFiles, packageType: SIP);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void Noark4_DiasTarInput_ProducesValidAIP()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Noark4", "diasPackage", "ffb1fda0-5b13-478f-9e4a-d68e8a944399.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Noark4, expectedContentFiles, packageType: AIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark5_DirectoryInput_ProducesValidSIP()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "Noark5", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.Noark5, expectedContentPaths, packageType: SIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark5_DirectoryInput_ProducesValidAIP()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "Noark5", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.Noark5, expectedContentPaths, packageType: AIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark5_DiasTarInput_ProducesValidSIP()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Noark5", "diasPackage", "4b73981c-1fab-4d4c-91e7-fcc6a3bc057f.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Noark5, expectedContentFiles, packageType: SIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Noark5_DiasTarInput_ProducesValidAIP()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Noark5", "diasPackage", "4b73981c-1fab-4d4c-91e7-fcc6a3bc057f.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Noark5, expectedContentFiles, packageType: AIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SpecializedSystem_DirectoryInput_ProducesValidSIP()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "SpecializedSystem", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.SpecializedSystem, expectedContentPaths, packageType: SIP);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void SpecializedSystem_DirectoryInput_ProducesValidAIP()
    {
        DirectoryInfo extractionDirectory = TestData.Directory("Archives", "SpecializedSystem", "extraction");

        string[] expectedContentPaths = GetPathsAsWhenInTar(extractionDirectory);

        RunScenario(extractionDirectory, ArchiveType.SpecializedSystem, expectedContentPaths, packageType: AIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void SpecializedSystem_DiasTarInput_ProducesValidSIP()
    {
        FileInfo diasTarFile = TestData.File("Archives", "SpecializedSystem", "diasPackage",
            "bf193afe-4483-4481-b457-e9ba4f19681c.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.SpecializedSystem, expectedContentFiles, packageType: SIP);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void SpecializedSystem_DiasTarInput_ProducesValidAIP()
    {
        FileInfo diasTarFile = TestData.File("Archives", "SpecializedSystem", "diasPackage",
            "bf193afe-4483-4481-b457-e9ba4f19681c.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.SpecializedSystem, expectedContentFiles, packageType: AIP);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Siard_FileInput_ProducesValidSIP()
    {
        FileInfo siardFile = TestData.File("Archives", "Siard", "extraction", "dbptk.siard");

        DirectoryInfo directoryWithExpectedContentFiles = TestData.Directory("Archives", "Siard", "extraction");

        const string unReferencedFile =
            "t01bclob12_dbptk-desktop-2.5.9_ext.siard_lobseg_1/content/schema1/table1/lob9/unreferenced-file.bin";

        string[] expectedContentFilePaths = GetPathsAsWhenInTar(directoryWithExpectedContentFiles, filePathsOnly: true)
            .Except([unReferencedFile]).ToArray();

        RunScenario(siardFile, ArchiveType.Siard, expectedContentFilePaths, packageType: SIP);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void Siard_FileInput_ProducesValidAIP()
    {
        FileInfo siardFile = TestData.File("Archives", "Siard", "extraction", "dbptk.siard");

        DirectoryInfo directoryWithExpectedContentFiles = TestData.Directory("Archives", "Siard", "extraction");

        const string unReferencedFile =
            "t01bclob12_dbptk-desktop-2.5.9_ext.siard_lobseg_1/content/schema1/table1/lob9/unreferenced-file.bin";

        string[] expectedContentFilePaths = GetPathsAsWhenInTar(directoryWithExpectedContentFiles, filePathsOnly: true)
            .Except([unReferencedFile]).ToArray();

        RunScenario(siardFile, ArchiveType.Siard, expectedContentFilePaths, packageType: AIP);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void Siard_DiasTarInput_ProducesValidSIP()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Siard", "diasPackage", "841c0a18-7308-4407-b421-3efaa420d891.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Siard, expectedContentFiles, packageType: SIP);
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public void Siard_DiasTarInput_ProducesValidAIP()
    {
        FileInfo diasTarFile =
            TestData.File("Archives", "Siard", "diasPackage", "841c0a18-7308-4407-b421-3efaa420d891.tar");

        string[] expectedContentFiles = DiasTarArchiveUtility.GetContentFileList(diasTarFile.FullName);

        RunScenario(diasTarFile, ArchiveType.Siard, expectedContentFiles, packageType: AIP);
    }

    private void RunScenario(FileSystemInfo input, ArchiveType archiveType, string[] expectedContentFiles,
        PackageType packageType, [CallerMemberName] string callerMemberName = null)
    {
        DirectoryInfo isolatedTemporaryDirectory = fixture.CreateIsolatedDirectory<ArkadeCoreApiTest>(callerMemberName);
        
        ArkadeProcessingArea.Establish(isolatedTemporaryDirectory.FullName);

        using var arkade = new Core.Base.Arkade();

        // 1. Load Archive
        Archive archive = arkade.LoadArchiveExtraction(input, archiveType);
        archive.ArchiveType.Should().Be(archiveType);

        // Instantiate OutputDiasPackage to generate the package ID expected in test reports
        var archiveMetadata = new ArchiveMetadata();
        archive.OutputDiasPackage = new OutputDiasPackage(packageType, archiveMetadata, archive.ProcessingDirectory);
        Uuid packageId = archive.OutputDiasPackage.Id;
        
        // 2. Create Test Session and 3. Run Tests (if supported)
        string[] expectedTestReportFileNames = []; 
        if (archive.IsTestable(out _))
        {
            archive.TestSession = arkade.CreateTestSession(archive);
            archive.TestSession.OutputLanguage = OutputLanguage;
            arkade.RunTests(archive);
            archive.TestSession.TestSuite.Should().NotBeNull();

            // Test generation of test reports exported
            arkade.GenerateTestReport(archive, isolatedTemporaryDirectory, 100, archive.InputDiasPackage);
            
            string baseReportsDirectoryName =
                OutputFileNames.StandaloneTestReportDirectory[
                    ..OutputFileNames.StandaloneTestReportDirectory.LastIndexOf('_')]; // Package id / timestamp cut off
            
            string reportsDirectoryPathStart = Path.Combine(isolatedTemporaryDirectory.FullName, baseReportsDirectoryName);
            
            string testReportExportDirectoryPath = Directory.GetDirectories(isolatedTemporaryDirectory.FullName)
                .Single(dir => dir.StartsWith(reportsDirectoryPathStart));
                
            IEnumerable<string> exportedTestReportFilePaths = Directory.GetFiles(testReportExportDirectoryPath);

            exportedTestReportFilePaths.Count().Should().Be(archiveType == ArchiveType.Siard ? 5 : 4); // (dbptk report)

            foreach (TestReportFormat testReportFormat in Enum.GetValues<TestReportFormat>())
            {
                string baseReportFileName =
                    OutputFileNames.StandaloneTestReportFile[
                        ..OutputFileNames.StandaloneTestReportFile.LastIndexOf('_')]; // Package id / timestamp cut off

                string reportFilePathStart = Path.Combine(testReportExportDirectoryPath, baseReportFileName);
                
                exportedTestReportFilePaths.Should().Contain(testReportFilePath =>
                    testReportFilePath.StartsWith(reportFilePathStart) &&
                    testReportFilePath.EndsWith(testReportFormat.ToString()));
            }

            if (archiveType == ArchiveType.Siard)
            {
                string dbptkValidationReportFilePath =
                    Path.Combine(testReportExportDirectoryPath, OutputFileNames.DbptkValidationReportFile);
                
                exportedTestReportFilePaths.Should().Contain(path => path.Equals(dbptkValidationReportFilePath));
            }
            
            // Prepare test report file paths to expect within the package being created
            string[] arkadeTestReportFileNames = Enum.GetValues<TestReportFormat>().Select(format =>
                packageType == SIP
                    ? string.Format(OutputFileNames.StandaloneTestReportFile, packageId, format)
                    : string.Format(OutputFileNames.TestReportFile, format)).ToArray();

            expectedTestReportFileNames = archiveType == ArchiveType.Siard
                ? [..arkadeTestReportFileNames, OutputFileNames.DbptkValidationReportFile]
                : arkadeTestReportFileNames;
        }

        // 4. Create Package
        string outputDirectory = isolatedTemporaryDirectory.CreateSubdirectory("output").FullName;

        arkade.CreatePackage(archive, OutputLanguage, generateFileFormatInfo: false, outputDirectory);

        // 5. Verify Package creation results
        VerifyPackage(archive, outputDirectory, expectedContentFiles, expectedTestReportFileNames);
    }

    private static void VerifyPackage(Archive archive, string outputDirectory, string[] expectedContentFilePaths,
        string[] expectedTestReportFileNames)
    {
        Uuid packageId = archive.OutputDiasPackage.Id;
        PackageType packageType = archive.OutputDiasPackage.PackageType;
        string resultsDirectoryName = string.Format(OutputFileNames.ResultOutputDirectory, packageId);
        string resultsDirectoryPath = Path.Combine(outputDirectory, resultsDirectoryName);

        var expectedResultFilePaths = new List<string>
        {
            Path.Combine(resultsDirectoryPath, $"{packageId}.tar"),
            Path.Combine(resultsDirectoryPath, $"{packageId}.xml"),
        };
        
        if(packageType == SIP)
        {
            string standaloneReportsDirectory = string.Format(OutputFileNames.StandaloneTestReportDirectory, packageId);

            IEnumerable<string> standAloneTestReportFilePaths = expectedTestReportFileNames.Select(testReportFileName =>
                Path.Combine(resultsDirectoryPath, standaloneReportsDirectory, testReportFileName));

            expectedResultFilePaths.AddRange(standAloneTestReportFilePaths);
        }
        
        // Verify files in the result directory
        List<string> resultFilePaths = Directory.GetFiles(resultsDirectoryPath, "*", SearchOption.AllDirectories).ToList();

        // Use to examine the actual difference between produced and expected result files:
        //IEnumerable<string> filesInResultsNotExpected = resultFilePaths.ExceptOnce(expectedResultFilePaths);
        //IEnumerable<string> expectedFilesNotInResults = expectedResultFilePaths.ExceptOnce(resultFilePaths);
        //filesInResultsNotExpected.Should().BeEmpty();
        //expectedFilesNotInResults.Should().BeEmpty();

        resultFilePaths.Should().BeEquivalentTo(expectedResultFilePaths);

        // Verify package file contents
        string tarFilePath = Path.Combine(resultsDirectoryPath, $"{packageId}.tar");

        List<string> packageFileList = DiasTarArchiveUtility.GetFileList(tarFilePath)
            .Select(Path.TrimEndingDirectorySeparator).ToList();

        List<string> expectedPackageFileList = CreateExpectedPackageFileList(
            archive.ArchiveType, packageId, expectedContentFilePaths, expectedTestReportFileNames, packageType)
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

    private static string[] CreateExpectedPackageFileList(ArchiveType archiveType, Uuid packageId,
        string[] expectedContentFilePaths, string[] expectedTestReportFileNames, PackageType packageType)
    {
        var packageRootDirectoryName = packageId.ToString();

        IEnumerable<string> arkadeAppliedPackageFilePaths = DiasTarArchiveUtility.GetArkadeAppliedPackageFilesList(archiveType, packageType)
            .Select(arkadeAppliedAipFilePath => $"{packageRootDirectoryName}/{arkadeAppliedAipFilePath}");

        IEnumerable<string> contentFilesPaths = expectedContentFilePaths.Select(contentFileName =>
            $"{packageRootDirectoryName}/{DirectoryNameContent}/{contentFileName}");

        if (packageType == SIP) // Test reports are expected outside the package
        {
            var administrativeMetadataDirectoryPath = $"{packageRootDirectoryName}/{DirectoryNameAdministrativeMetadata}";

            IEnumerable<string> expectedIrregularFilesPaths = archiveType switch
            {
                // These files should, according to the specification, not be part of SIP packages. Nevertheless, Arkade has
                // included them for a long time, so we expect them in these integration tests until the practice is clarified.
                
                ArchiveType.Noark5 when packageType == SIP =>
                [
                    $"{administrativeMetadataDirectoryPath}/{ArkivuttrekkXmlFileName}",
                    $"{administrativeMetadataDirectoryPath}/{AddmlXsdFileName}"
                ],
                ArchiveType.SpecializedSystem when packageType == SIP =>
                [
                    $"{administrativeMetadataDirectoryPath}/{AddmlXmlFileName}",
                    $"{administrativeMetadataDirectoryPath}/{AddmlXsdFileName}"
                ],
                ArchiveType.Siard when packageType == SIP =>
                [
                    $"{administrativeMetadataDirectoryPath}/{SiardMetadataXmlFileName}",
                    $"{administrativeMetadataDirectoryPath}/{SiardMetadataXsdFileName}"
                ],
                _ => []
            };

            return [packageRootDirectoryName, .. arkadeAppliedPackageFilePaths, .. contentFilesPaths, .. expectedIrregularFilesPaths];
        }

        if (expectedTestReportFileNames.Length == 0)
            return [packageRootDirectoryName, .. arkadeAppliedPackageFilePaths, .. contentFilesPaths];
        
        string repositoryOperationsDirectory =
            $"{packageRootDirectoryName}/{DirectoryNameAdministrativeMetadata}" +
            $"/{DirectoryNameRepositoryOperations}";

        string testReportDirectory = $"{repositoryOperationsDirectory}/{OutputFileNames.TestReportDirectory}";

        IEnumerable<string> testReportFilePaths =
        [
            testReportDirectory,
            .. expectedTestReportFileNames.Select(testReportFileName => $"{testReportDirectory}/{testReportFileName}")
        ];

        // The test-session log ships in the AIP's repository_operations, beside the test reports.
        string arkadeLogFilePath = $"{repositoryOperationsDirectory}/{ArkadeXmlLogFileName}";

        return [packageRootDirectoryName, .. arkadeAppliedPackageFilePaths, .. contentFilesPaths, .. testReportFilePaths, arkadeLogFilePath];
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
