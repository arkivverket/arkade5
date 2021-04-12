using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Hardcoded;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using Arkivverket.Arkade.Core.Tests.Testing.Noark5;
using FluentAssertions;
using System.IO;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes.Hardcoded
{
    public class AH_01_ControlChecksumTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportNotReportCorrectChecksumValue()
        {
            string expectedSha256Hash = "3b67b886241a7d63798ea0e63a70cbca6c2109ab3962b2a5815cbadcb90cbb08";

            AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithChecksum(new Checksum("SHA-256", expectedSha256Hash))
                .WithFileInfo(new FileInfo(Path.Combine(TestUtil.TestDataDirectory, "checksum.txt")))
                .Build();

            FlatFile flatFile = new FlatFile(flatFileDefinition);

            AH_01_ControlChecksum test = new AH_01_ControlChecksum();
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
                .WithFileInfo(new FileInfo(Path.Combine(TestUtil.TestDataDirectory, "checksum.txt")))
                .Build();

            FlatFile flatFile = new FlatFile(flatFileDefinition);

            AH_01_ControlChecksum test = new AH_01_ControlChecksum();
            test.Run(flatFile);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(flatFileDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should()
                .Contain("Forventet sjekksum: invalid").And
                .Contain("Aktuell sjekksum: " + expectedSha256Hash);
        }

        [Fact]
        public void ShouldReportInvalidChecksumAlgorithm()
        {
            string expectedSha256Hash = "3b67b886241a7d63798ea0e63a70cbca6c2109ab3962b2a5815cbadcb90cbb08".ToUpper();

            AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithChecksum(new Checksum("invalid hash", expectedSha256Hash))
                .WithFileInfo(new FileInfo(Path.Combine(TestUtil.TestDataDirectory, "checksum.txt")))
                .Build();

            FlatFile flatFile = new FlatFile(flatFileDefinition);

            AH_01_ControlChecksum test = new AH_01_ControlChecksum();
            test.Run(flatFile);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(flatFileDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ukjent sjekksum-algoritme: invalid hash");
        }
    }
}