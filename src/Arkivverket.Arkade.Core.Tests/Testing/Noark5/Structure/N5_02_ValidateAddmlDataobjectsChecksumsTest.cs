using System;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5.Structure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5.Structure
{
    public class N5_02_ValidateAddmlDataobjectsChecksumsTest : LanguageDependentTest
    {

        private readonly ITestOutputHelper _output;

        public N5_02_ValidateAddmlDataobjectsChecksumsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldValidateThatAllChecksumsAreCorrect()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\correct";
            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateAddmlDataobjectsChecksums = new N5_02_ValidateAddmlDataobjectsChecksums();
            validateAddmlDataobjectsChecksums.Test(archive);
            var testRun = validateAddmlDataobjectsChecksums.GetTestRun();
            testRun.TestResults.GetNumberOfResults().Should().Be(9);
            testRun.IsSuccess().Should().BeTrue();

            foreach (var testResult in testRun.TestResults.TestsResults)
            {
                _output.WriteLine(testResult.Location + ": " + testResult.Message);
            }
        }
        [Fact]
        public void HasErrorChecksum()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\errors";
            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateAddmlDataobjectsChecksums = new N5_02_ValidateAddmlDataobjectsChecksums();
            validateAddmlDataobjectsChecksums.Test(archive);
            var testRun = validateAddmlDataobjectsChecksums.GetTestRun();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.IsSuccess().Should().BeFalse();
        }

        [Fact]
        public void HasMissingChecksumValue()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\missingValue";
            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateAddmlDataobjectsChecksums = new N5_02_ValidateAddmlDataobjectsChecksums();
            validateAddmlDataobjectsChecksums.Test(archive);
            var testRun = validateAddmlDataobjectsChecksums.GetTestRun();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.IsSuccess().Should().BeFalse();
        }

        [Fact]
        public void HasMissingChecksumAlgorithm()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\missingAlgorithm";
            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateAddmlDataobjectsChecksums = new N5_02_ValidateAddmlDataobjectsChecksums();
            validateAddmlDataobjectsChecksums.Test(archive);
            var testRun = validateAddmlDataobjectsChecksums.GetTestRun();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Message.Should()
                .Be("Sjekksumalgoritme for filen 'arkivstruktur.xml' ble ikke funnet.");
            testRun.IsSuccess().Should().BeFalse();
        }


        [Fact]
        public void HasInvalidChecksumAlgorithm()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\unsupportedAlgorithm";
            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateAddmlDataobjectsChecksums = new N5_02_ValidateAddmlDataobjectsChecksums();
            validateAddmlDataobjectsChecksums.Test(archive);
            var testRun = validateAddmlDataobjectsChecksums.GetTestRun();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Message.Should()
                .Be("Oppgitt sjekksumalgoritme ('SHA1') for 'arkivstruktur.xml' er ikke støttet av Arkade.");
            testRun.IsSuccess().Should().BeFalse();
        }

        [Fact]
        public void HasMissingChecksumProperty()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\missingProperty";
            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateAddmlDataobjectsChecksums = new N5_02_ValidateAddmlDataobjectsChecksums();
            validateAddmlDataobjectsChecksums.Test(archive);
            var testRun = validateAddmlDataobjectsChecksums.GetTestRun();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Message.Should()
                .Be("Klarte ikke finne <property name=\"checksum\"> for filen 'arkivstruktur.xml'.");
            testRun.IsSuccess().Should().BeFalse();
        }

        [Fact]
        public void TargetFileIsMissing()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\missingTargetFile";
            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateAddmlDataobjectsChecksums = new N5_02_ValidateAddmlDataobjectsChecksums();
            validateAddmlDataobjectsChecksums.Test(archive);
            var testRun = validateAddmlDataobjectsChecksums.GetTestRun();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Message.Should()
                .Be("Filen arkivstruktur.xml ble ikke funnet");
            testRun.IsSuccess().Should().BeFalse();
        }
    }
}