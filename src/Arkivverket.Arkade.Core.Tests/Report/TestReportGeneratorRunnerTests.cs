using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Report;

public class TestReportGeneratorRunnerTests(TestSessionLifeTimeFilesFixture fixture)
{
    [Fact]
    public void GetExtensionReadyTestReportFullNameTest_SIP_creation()
    {
        DirectoryInfo isolatedDirectory = fixture.CreateIsolatedDirectory<TestReportGeneratorRunnerTests>();
        DirectoryInfo processingDirectory = isolatedDirectory.CreateSubdirectory("processing");
        DirectoryInfo outputDirectory = isolatedDirectory.CreateSubdirectory("output");

        var outputDiasPackage =
            new OutputDiasPackage(PackageType.SubmissionInformationPackage, new ArchiveMetadata(), processingDirectory);

        // Expected paths setup
        string expectedArkadeResultsDirectoryName = string.Format(OutputFileNames.ResultOutputDirectory, outputDiasPackage.Id);
        string expectedTestReportsDirectoryName = string.Format(OutputFileNames.StandaloneTestReportDirectory, outputDiasPackage.Id);
        string expectedTestReportsDirectoryPath = Path.Combine(outputDirectory.FullName, expectedArkadeResultsDirectoryName, expectedTestReportsDirectoryName);
        string expectedTestReportFileName = string.Format(OutputFileNames.StandaloneTestReportFile, outputDiasPackage.Id, "{0}");
        string expectedExtensionReadyTestReportFilePath = Path.Combine(expectedTestReportsDirectoryPath, expectedTestReportFileName);
        
        // Run
        string extensionReadyTestReportFilePath = TestReportGeneratorRunner.GetExtensionReadyTestReportFullName(
            outputDirectory, outputDiasPackage, default, out string testReportsDirectoryPath);
        
        // Test
        testReportsDirectoryPath.Should().Be(expectedTestReportsDirectoryPath);
        extensionReadyTestReportFilePath.Should().Be(expectedExtensionReadyTestReportFilePath);
    }
    
    [Fact]
    public void GetExtensionReadyTestReportFullNameTest_AIP_creation()
    {
        DirectoryInfo isolatedDirectory = fixture.CreateIsolatedDirectory<TestReportGeneratorRunnerTests>();
        DirectoryInfo processingDirectory = isolatedDirectory.CreateSubdirectory("processing");

        const PackageType packageType = PackageType.ArchivalInformationPackage;

        var outputDiasAip = new OutputDiasPackage(packageType, new ArchiveMetadata(), processingDirectory);

        // Expected paths setup
        var expectedTestReportsDirectoryPath = outputDiasAip.WorkingDirectory.RepositoryOperations().WithSubDirectory(OutputFileNames.TestReportDirectory).ToString();
        string expectedExtensionReadyTestReportFilePath = Path.Combine(expectedTestReportsDirectoryPath, OutputFileNames.TestReportFile);

        // Run
        string extensionReadyTestReportFilePath = TestReportGeneratorRunner.GetExtensionReadyTestReportFullName(null, outputDiasAip, default, out string testReportsDirectoryPath); // Droppe outputDirectory?

        // Test
        testReportsDirectoryPath.Should().Be(expectedTestReportsDirectoryPath);
        extensionReadyTestReportFilePath.Should().Be(expectedExtensionReadyTestReportFilePath);
    }
    
    [Fact]
    public void GetExtensionReadyTestReportFullNameTest_Export_ExtractionInput()
    {
        DirectoryInfo isolatedDirectory = fixture.CreateIsolatedDirectory<TestReportGeneratorRunnerTests>();
        DirectoryInfo outputDirectory = isolatedDirectory.CreateSubdirectory("output");

        var timeOfTesting = DateTime.Now;
        var nowTimestamp = timeOfTesting.ToString("yyyyMMddHHmmss");

        // Expected paths setup
        string expectedTestReportsDirectoryName = string.Format(OutputFileNames.StandaloneTestReportDirectory, nowTimestamp);
        string expectedTestReportsDirectoryPath = Path.Combine(outputDirectory.FullName, expectedTestReportsDirectoryName);
        string expectedExtensionReadyTestReportFileName = string.Format(OutputFileNames.StandaloneTestReportFile, nowTimestamp, "{0}");
        string expectedExtensionReadyTestReportFilePath = Path.Combine(expectedTestReportsDirectoryPath, expectedExtensionReadyTestReportFileName);
        
        // Run
        string extensionReadyTestReportFilePath = TestReportGeneratorRunner.GetExtensionReadyTestReportFullName(outputDirectory, null, timeOfTesting, out string testReportsDirectoryPath);
        
        // Test
        testReportsDirectoryPath.Should().Be(expectedTestReportsDirectoryPath);
        extensionReadyTestReportFilePath.Should().Be(expectedExtensionReadyTestReportFilePath);
    }
    
    [Fact]
    public void GetExtensionReadyTestReportFullNameTest_Export_DiasInput()
    {
        DirectoryInfo isolatedDirectory = fixture.CreateIsolatedDirectory<TestReportGeneratorRunnerTests>();
        DirectoryInfo processingDirectory = isolatedDirectory.CreateSubdirectory("processing");
        DirectoryInfo outputDirectory = isolatedDirectory.CreateSubdirectory("output");

        Uuid inputIpUuid = Uuid.Random();

        // Set up minimal working directory for InputDiasPackage
        DirectoryInfo workingDirectoryRoot = processingDirectory.CreateSubdirectory(inputIpUuid.GetValue());
        File.WriteAllText(
            Path.Combine(workingDirectoryRoot.FullName, ArkadeConstants.DiasMetsXmlFileName),
            $"{{\"Id\": \"UUID:{inputIpUuid}\"}}"
        );
        var inputDiasPackage = new InputDiasPackage(new DiasPackageWorkingDirectory(workingDirectoryRoot), null, inputIpUuid.GetValue());

        // Expected paths setup
        string expectedTestReportsDirectoryName = string.Format(OutputFileNames.StandaloneTestReportDirectory, inputDiasPackage.Id);
        string expectedTestReportsDirectoryPath = Path.Combine(outputDirectory.FullName, expectedTestReportsDirectoryName);
        string expectedExtensionReadyTestReportFileName = string.Format(OutputFileNames.StandaloneTestReportFile, inputDiasPackage.Id, "{0}");
        string expectedExtensionReadyTestReportFilePath = Path.Combine(expectedTestReportsDirectoryPath, expectedExtensionReadyTestReportFileName);

        // Run
        string extensionReadyTestReportFilePath = TestReportGeneratorRunner.GetExtensionReadyTestReportFullName(outputDirectory, inputDiasPackage, default, out string testReportsDirectoryPath);

        // Test
        testReportsDirectoryPath.Should().Be(expectedTestReportsDirectoryPath);
        extensionReadyTestReportFilePath.Should().Be(expectedExtensionReadyTestReportFilePath);
    }
}
