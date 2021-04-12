using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_10_AnalyseFrequenceListTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldCreateFrequencyList()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder().Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_10_AnalyseFrequenceList test = new A_10_AnalyseFrequenceList();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "A"));
            test.Run(new Field(fieldDefinition, "A"));
            test.Run(new Field(fieldDefinition, "B"));
            test.Run(new Field(fieldDefinition, "B"));
            test.Run(new Field(fieldDefinition, "B"));
            test.Run(new Field(fieldDefinition, "C"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(3);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("2 forekomster av A");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("3 forekomster av B");
            testRun.Results[2].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[2].Message.Should().Be("1 forekomster av C");
        }
    }
}