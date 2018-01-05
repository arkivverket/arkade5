using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes.Hardcoded;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using Arkivverket.Arkade.Test.Tests.Noark5;
using FluentAssertions;
using System.IO;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlChecksumTest
    {
        [Fact]
        public void ShouldReportNotReportCorrectChecksumValue()
        {
            string expectedSha256Hash = "3b67b886241a7d63798ea0e63a70cbca6c2109ab3962b2a5815cbadcb90cbb08".ToUpper();

            AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithChecksum(new Checksum("SHA-256", expectedSha256Hash))
                .WithFileInfo(new FileInfo(TestUtil.TestDataDirectory + "checksum.txt"))
                .Build();

            FlatFile flatFile = new FlatFile(flatFileDefinition);

            ControlChecksum test = new ControlChecksum();
            test.Run(flatFile);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(0);
        }

        [Fact]
        public void ShouldReportInvalidChecksumValue()
        {
            string expectedSha256Hash = "3b67b886241a7d63798ea0e63a70cbca6c2109ab3962b2a5815cbadcb90cbb08".ToUpper();

            AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithChecksum(new Checksum("SHA-256", "invalid"))
                .WithFileInfo(new FileInfo(TestUtil.TestDataDirectory + "checksum.txt"))
                .Build();

            FlatFile flatFile = new FlatFile(flatFileDefinition);

            ControlChecksum test = new ControlChecksum();
            test.Run(flatFile);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(flatFileDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Forventet checksum 'invalid'. Var '" + expectedSha256Hash + "'.");
        }

        [Fact]
        public void ShouldReportInvalidChecksumAlgorithm()
        {
            string expectedSha256Hash = "3b67b886241a7d63798ea0e63a70cbca6c2109ab3962b2a5815cbadcb90cbb08".ToUpper();

            AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithChecksum(new Checksum("invalid hash", expectedSha256Hash))
                .WithFileInfo(new FileInfo(TestUtil.TestDataDirectory + "checksum.txt"))
                .Build();

            FlatFile flatFile = new FlatFile(flatFileDefinition);

            ControlChecksum test = new ControlChecksum();
            test.Run(flatFile);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(flatFileDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ukjent algoritme 'invalid hash'");
        }
    }
}