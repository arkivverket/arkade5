using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5.Structure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Test.Tests.Noark5.Structure
{
    public class ValidateXmlWithSchemaTest
    {
        public ValidateXmlWithSchemaTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        private static TestRun ValidateArchive(string workingDirectory)
        {
            var archiveExtraction = new Archive("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;

   //         var testResults = new ValidateXmlWithSchema().RunTest(archiveExtraction);
            return null;
        }

        [Fact(Skip = "must refactor test")]
        public void ShouldReturnErrorsWhenXmlIsInvalid()
        {
            // line 42 has invalid element

            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureValidation\\error";
            var testResults = ValidateArchive(workingDirectory);

            _output.WriteLine(testResults.ToString());

            testResults.IsSuccess().Should().BeFalse();
        }

        [Fact(Skip = "must refactor test")]
        public void ShouldReturnWithNoErrorsWhenXmlIsValid()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureValidation\\correct";
            var testResults = ValidateArchive(workingDirectory);

            _output.WriteLine(testResults.ToString());

            testResults.IsSuccess().Should().BeTrue();
        }
    }
}