using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlMaxLengthTest
    {
        [Fact]
        public void ShouldReportValuesShorterThanMinLength()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithMaxLength(3)
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            ControlMaxLength test = new ControlMaxLength();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, ""));
            test.Run(new Field(fieldDefinition, "1"));
            test.Run(new Field(fieldDefinition, "12"));
            test.Run(new Field(fieldDefinition, "123"));
            test.Run(new Field(fieldDefinition, "1234"));
            test.Run(new Field(fieldDefinition, "1234"));
            test.Run(new Field(fieldDefinition, "12345"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Verdier lengre enn maksimumlengde: 1234 12345");
        }
    }
}