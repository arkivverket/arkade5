using System;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5.Structure;
using Arkivverket.Arkade.Core.Tests.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5.Structure
{
    public class N5_03_ValidateXmlWithSchemaTest
    {
        [Fact]
        public void XmlFilesAreValidAccordingToSchema()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureValidation\\correct";

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.IsSuccess().Should().BeTrue();
        }

        [Fact]
        public void XmlFilesAreNotValidAccordingToSchema()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureValidation\\error";

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.TestResults.TestsResults.Should().Contain(r => r.IsError());
        }

        [Fact]
        public void XmlFilesAreValidAccordingToCustomSchema()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureValidation\\custom";

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.IsSuccess().Should().BeTrue();
        }

        [Fact]
        public void XmlFilesAreNotValidAccordingToCustomSchema()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureValidation\\customError";

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.TestResults.TestsResults.Should().Contain(r => r.IsError());
        }

        private static TestRun CreateTestRun(string workingDirectory)
        {
            Archive archive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory)
                .Build();

            var validateXmlWithSchema = new N5_03_ValidateXmlWithSchema();

            validateXmlWithSchema.Test(archive);

            return validateXmlWithSchema.GetTestRun();
        }


    }
}