using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5.Structure;
using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5.Structure
{
    public class ValidateStructureFileExistsTest
    {
        private readonly ITestOutputHelper _output;

        public ValidateStructureFileExistsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldValidateThatAllListedFilesExist()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureValidationFileExists\\AllListedFilesExists";

            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent($"{workingDirectory}\\arkiv")
                .Build();

            var validateStructureFileExists = new N5_01_ValidateStructureFileExists();
            validateStructureFileExists.Test(archive);
            var testRun = validateStructureFileExists.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();

        }

        [Fact]
        public void OneFileIsMissing()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureValidationFileExists\\OneFileIsMissing";

            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent($"{workingDirectory}\\arkiv")
                .Build();

            var validateStructureFileExists = new N5_01_ValidateStructureFileExists();
            validateStructureFileExists.Test(archive);
            var testRun = validateStructureFileExists.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();

        }


    }
}
