using System.IO;
using System.Reflection;
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

        [Fact]
        public void ShouldValidateThatAllChecksumsAreCorrect()
        {
            string workingDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/TestData/Noark5/StructureChecksums/correct";
            var archive = new Core.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "/content")
                .Build();

            var validateAddmlDataobjectsChecksums = new ValidateAddmlDataobjectsChecksums();
            validateAddmlDataobjectsChecksums.Test(archive);
            var testRun = validateAddmlDataobjectsChecksums.GetTestRun();
            testRun.Results.Count.Should().Be(2);
            testRun.IsSuccess().Should().BeTrue();

            foreach (var testResult in testRun.Results)
            {
                _output.WriteLine(testResult.Location + ": " + testResult.Message);
            }
        }
    }
}