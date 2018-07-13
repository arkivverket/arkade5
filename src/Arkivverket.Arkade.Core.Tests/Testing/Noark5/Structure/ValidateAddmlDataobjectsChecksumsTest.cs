using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Testing.Noark5.Structure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5.Structure
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
            string workingDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\TestData\\Noark5\\StructureChecksums\\correct";
            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateAddmlDataobjectsChecksums = new ValidateAddmlDataobjectsChecksums();
            validateAddmlDataobjectsChecksums.Test(archive);
            var testRun = validateAddmlDataobjectsChecksums.GetTestRun();
            testRun.Results.Count.Should().Be(9);
            testRun.IsSuccess().Should().BeTrue();

            foreach (var testResult in testRun.Results)
            {
                _output.WriteLine(testResult.Location + ": " + testResult.Message);
            }
        }
        [Fact]
        public void HasErrorChecksum()
        {
            string workingDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\TestData\\Noark5\\StructureChecksums\\errors";
            var archive = new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateAddmlDataobjectsChecksums = new ValidateAddmlDataobjectsChecksums();
            System.Exception ex = Assert.Throws<ArkadeException>(() => validateAddmlDataobjectsChecksums.Test(archive));
        }
    }
}