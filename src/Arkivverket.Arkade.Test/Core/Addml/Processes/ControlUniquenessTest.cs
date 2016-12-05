using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlUniquenessTest
    {
        [Fact]
        public void ShouldReportUniquenessIfAllFieldsAreUniqe()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            ControlUniqueness test = new ControlUniqueness();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "A"));
            test.Run(new Field(fieldDefinition, "B"));
            test.Run(new Field(fieldDefinition, "C"));
            test.Run(new Field(fieldDefinition, "D"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Alle verdier er unike");
        }

        [Fact]
        public void ShouldReportNonUniquenessIfTwoFieldsHaveSameValue()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            ControlUniqueness test = new ControlUniqueness();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "A"));
            test.Run(new Field(fieldDefinition, "B"));
            test.Run(new Field(fieldDefinition, "C"));
            test.Run(new Field(fieldDefinition, "A"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Verdiene er ikke unike");
        }

    }
}