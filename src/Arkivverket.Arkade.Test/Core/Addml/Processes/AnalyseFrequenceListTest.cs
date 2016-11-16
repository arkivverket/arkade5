using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class AnalyseFrequenceListTest
    {
        [Fact]
        public void ShouldCreateFrequencyList()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder().Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            AnalyseFrequenceList test = new AnalyseFrequenceList();
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