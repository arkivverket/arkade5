using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class AnalyseFindExtremeValuesTest
    {
        [Fact]
        public void ShouldFindExtremeValues()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder().Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            AnalyseFindExtremeValues test = new AnalyseFindExtremeValues();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "1234567890"));
            test.Run(new Field(fieldDefinition, "12345"));
            test.Run(new Field(fieldDefinition, "1"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetFieldIndeks().ToString());
            testRun.Results[0].Message.Should().Be("Lengste/korteste verdi: 10/1");
        }
    }
}