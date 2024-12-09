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
    public class AH_02_ControlExtraOrMissingFilesTest : LanguageDependentTest
    {

        [Fact]
        public void ShouldReportWhenFileInArchiveIsNotReferencedInAddml()
        {
            WorkingDirectory workingDirectory = new WorkingDirectory(null, new DirectoryInfo(Path.Combine(TestUtil.TestDataDirectory, "noark3")));

            AddmlFlatFileDefinition flatFileDefinition1 = new AddmlFlatFileDefinitionBuilder()
                .WithFileInfo(new FileInfo(Path.Combine(workingDirectory.Content().DirectoryInfo().FullName, "nosuchfile.txt")))
                .WithRelativeFileName("nosuchfile.txt")
                .Build();

            AddmlFlatFileDefinition flatFileDefinition2 = new AddmlFlatFileDefinitionBuilder()
                .WithFileInfo(new FileInfo(Path.Combine(workingDirectory.Content().DirectoryInfo().FullName, "ARKIV.DAT")))
                .WithRelativeFileName("ARKIV.DAT")
                .Build();

            AddmlDefinition addmlDefinition = new AddmlDefinitionBuilder()
                .WithAddmlFlatFileDefinitions(
                    new List<AddmlFlatFileDefinition> {
                        flatFileDefinition1,
                        flatFileDefinition2},
                    new List<AddmlFlatFileDefinition> {
                        flatFileDefinition1,
                 })
                .Build();

            Archive archive = new Archive(ArchiveType.Fagsystem, null, Uuid.Random(), workingDirectory, null); // NB! UUID-origin

            AH_02_ControlExtraOrMissingFiles test = new AH_02_ControlExtraOrMissingFiles(addmlDefinition, archive);

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(4);
            string errorMessage = "Finnes i arkiv, men ikke i ADDML";
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Contain("DOK.DAT");
            testRun.TestResults.TestsResults[0].Message.Should().Be(errorMessage);
            testRun.TestResults.TestsResults[1].Location.ToString().Should().Contain("noark_3_arkivuttrekk_med_prosesser.xml");
            testRun.TestResults.TestsResults[1].Message.Should().Be(errorMessage);
            testRun.TestResults.TestsResults[2].Location.ToString().Should().Contain("SAK.DAT");
            testRun.TestResults.TestsResults[2].Message.Should().Be(errorMessage);
            testRun.TestResults.TestsResults[3].Location.ToString().Should().Contain("nosuchfile.txt");
            testRun.TestResults.TestsResults[3].Message.Should().Be("Finnes i ADDML, men ikke i arkiv");
        }
    }
}