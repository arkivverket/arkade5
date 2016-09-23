using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using Arkivverket.Arkade.Tests.Noark5.Structure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Test.Tests.Noark5.Structure
{
    public class ValidateAddmlDataobjectsChecksumsTest
    {

        private readonly ITestOutputHelper _output;

        public ValidateAddmlDataobjectsChecksumsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "Skip tests until we figure out why the checksums are different on various machines and setup.")]
        public void ShouldValidateThatAllChecksumsAreCorrect()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\correct";
            var archiveExtraction = new Archive("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;
 /*           
            var testResults = new ValidateAddmlDataobjectsChecksums().RunTest(archiveExtraction);

            foreach (var testResult in testResults.Results)
            {
                _output.WriteLine(testResult.Message);
            }

            testResults.Results.Count.Should().Be(2);

            testResults.IsSuccess().Should().BeTrue();
            */
        }
    }
}