using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5.Structure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Test.Tests.Noark5.Structure
{
    public class ValidateChecksumsTest
    {

        private readonly ITestOutputHelper _output;

        public ValidateChecksumsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldValidateThatAllChecksumsAreCorrect()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\StructureChecksums\\correct";
            var archiveExtraction = new ArchiveExtraction("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;
            
            var testResults = new ValidateChecksums().RunTest(archiveExtraction);

            foreach (var testResult in testResults.Results)
            {
                _output.WriteLine(testResult.Message);
            }

            testResults.Results.Count.Should().Be(2);

            testResults.IsSuccess().Should().BeTrue();

        }
    }
}