using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_18_ControlMaxLengthTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportValuesShorterThanMinLength()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithMaxLength(3)
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_18_ControlMaxLength test = new A_18_ControlMaxLength();
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