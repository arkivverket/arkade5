using System;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests;
using Arkivverket.Arkade.Core.Tests.Noark5.Structure;
using Arkivverket.Arkade.Test.Base;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Test.Tests.Noark5.Structure
{
    public class ValidateXmlWithSchemaTest
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

            testRun.Results.Should().Contain(r => r.IsError());
        }

        private static TestRun CreateTestRun(string workingDirectory)
        {
            Archive archive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory)
                .Build();

            var validateXmlWithSchema = new ValidateXmlWithSchema();

            validateXmlWithSchema.Test(archive);

            return validateXmlWithSchema.GetTestRun();
        }


    }
}