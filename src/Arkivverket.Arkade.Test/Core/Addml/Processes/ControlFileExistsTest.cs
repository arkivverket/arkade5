using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using Arkivverket.Arkade.Test.Tests.Noark5;
using FluentAssertions;
using System.IO;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlFileExistsTest
    {

        // TODO: lage arkiv som både har ekstra filer og mangler filer!


        [Fact]
        public void ShouldReportWhenFileDoesNotExists()
        {
            AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithFileName("nosuchfile.txt")
                .WithFileInfo(new FileInfo(TestUtil.TestDataDirectory + "nosuchfile.txt"))
                .Build();
            FlatFile flatFile = new FlatFile(flatFileDefinition);

            ControlFileExists test = new ControlFileExists();
            test.Run(flatFile);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(flatFileDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().MatchRegex("^Fil finnes ikke: .*nosuchfile.txt$");
        }

        [Fact(Skip = "TODO jostein")]
        public void ShouldReportWhenFileInArchiveIsNotReferencedInAddml()
        {
            AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithFileName("checksum.txt")
                .WithFileInfo(new FileInfo(TestUtil.TestDataDirectory + "checksum.txt"))
                .Build();
            FlatFile flatFile = new FlatFile(flatFileDefinition);



            //todo lage arkiv som både har ekstra filer og mangler filer!

            ControlFileExists test = new ControlFileExists();
            test.Run(flatFile);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(0);
            testRun.Results[0].Location.ToString().Should().Be(flatFileDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("dd");
        }

        [Fact]
        public void ShouldNotReportWhenFileExists()
        {
            AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithFileName("checksum.txt")
                .WithFileInfo(new FileInfo(TestUtil.TestDataDirectory + "checksum.txt"))
                .Build();
            FlatFile flatFile = new FlatFile(flatFileDefinition);

            ControlFileExists test = new ControlFileExists();
            test.Run(flatFile);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(0);
        }

    }
}