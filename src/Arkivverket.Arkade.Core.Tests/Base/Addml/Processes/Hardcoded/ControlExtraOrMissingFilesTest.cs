using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Hardcoded;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using Arkivverket.Arkade.Core.Tests.Testing.Noark5;
using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes.Hardcoded
{
    public class ControlExtraOrMissingFilesTest
    {

        [Fact]
        public void ShouldReportWhenFileInArchiveIsNotReferencedInAddml()
        {
            WorkingDirectory workingDirectory = new WorkingDirectory(null, new DirectoryInfo(TestUtil.TestDataDirectory + "noark3"));

            AddmlFlatFileDefinition flatFileDefinition1 = new AddmlFlatFileDefinitionBuilder()
                .WithFileInfo(new FileInfo(workingDirectory.Content().DirectoryInfo().FullName + Path.DirectorySeparatorChar + "nosuchfile.txt"))
                .WithFileName("nosuchfile.txt")
                .Build();

            AddmlFlatFileDefinition flatFileDefinition2 = new AddmlFlatFileDefinitionBuilder()
                .WithFileInfo(new FileInfo(workingDirectory.Content().DirectoryInfo().FullName + Path.DirectorySeparatorChar + "ARKIV.DAT"))
                .WithFileName("ARKIV.DAT")
                .Build();

            AddmlDefinition addmlDefinition = new AddmlDefinitionBuilder()
                .WithAddmlFlatFileDefinitions(new List<AddmlFlatFileDefinition> {
                        flatFileDefinition1,
                        flatFileDefinition2
                    })
                .Build();

            Archive archive = new Archive(ArchiveType.Fagsystem, Uuid.Random(), workingDirectory);

            ControlExtraOrMissingFiles test = new ControlExtraOrMissingFiles(addmlDefinition, archive);

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(4);
            string errorMessage = "Finnes i arkiv, men ikke i ADDML";
            testRun.Results[0].Location.ToString().Should().Contain("DOK.DAT");
            testRun.Results[0].Message.Should().Be(errorMessage);
            testRun.Results[1].Location.ToString().Should().Contain("noark_3_arkivuttrekk_med_prosesser.xml");
            testRun.Results[1].Message.Should().Be(errorMessage);
            testRun.Results[2].Location.ToString().Should().Contain("SAK.DAT");
            testRun.Results[2].Message.Should().Be(errorMessage);
            testRun.Results[3].Location.ToString().Should().Contain("nosuchfile.txt");
            testRun.Results[3].Message.Should().Be("Finnes i ADDML, men ikke i arkiv");
        }

        [Fact(Skip = "Initialization of Archive expects addml-file in workingdirectory")]
        public void ShouldNotReportExtraFilesWhenDefinedInDokversXml()
        {
            WorkingDirectory workingDirectory = new WorkingDirectory(null, new DirectoryInfo(TestUtil.TestDataDirectory + "noark4-extrafiles"));

            AddmlFlatFileDefinition flatFileDefinition2 = new AddmlFlatFileDefinitionBuilder()
                .WithFileInfo(new FileInfo(
                    workingDirectory.Content().DirectoryInfo().FullName 
                    + Path.DirectorySeparatorChar + "DOKVERS.XML"))
                .WithFileName("DATA\\DOKVERS.XML")
                .Build();

            AddmlDefinition addmlDefinition = new AddmlDefinitionBuilder()
                .WithAddmlFlatFileDefinitions(new List<AddmlFlatFileDefinition> {
                    flatFileDefinition2
                })
                .Build();

            Archive archive = new Archive(ArchiveType.Noark4, Uuid.Random(), workingDirectory);
            
            ControlExtraOrMissingFiles test = new ControlExtraOrMissingFiles(addmlDefinition, archive);

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
        }

    }
}